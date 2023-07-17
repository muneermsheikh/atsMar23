using System.Net.Http.Headers;
using System.Text.Json;
using api.Errors;
using api.Extensions;
using core.Dtos;
using core.Dtos.fiance;
using core.Entities.AccountsNFinance;
using core.Entities.Attachments;
using core.Entities.Identity;
using core.Interfaces;
using core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     [Authorize]
    public class FinanceController : BaseApiController
	{
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountsNFinanceServices _financeServices;
        private readonly int DefaultVoucherNo = 1001;

		public FinanceController(UserManager<AppUser> userManager, IAccountsNFinanceServices financeServices)
		{
            _financeServices = financeServices;
            _userManager = userManager;
			
		}

        [HttpGet("coaslist")]
        public async Task<ActionResult<ICollection<Coa>>> GetCOAList()
        {
            var coaslist = await _financeServices.GetCOAList();
            if (coaslist == null) return NotFound(new ApiResponse(404, "No Chart of account on record"));
            return Ok(coaslist);
        }

        [HttpGet("coas")]
        public async Task<Pagination<Coa>> GetChartOfAccounts([FromQuery]CoaParams coaParams)
        {
            var coas = await _financeServices.GetCOAs(coaParams);
            return coas;
        }

        [HttpGet("coasforpayment/{applicationno}")]
        public async Task<ICollection<Coa>> GetCOAsForPayment(int applicationno)
        {
            var coas = await _financeServices.GetCoasForPayments(applicationno);
            if(coas==null || coas.Count==0) return null;
            return coas;
        }

        [HttpGet("coaforcandidate/{applicationno}/{create}")]
        public async Task<CoaDto> COAForCandidate(int applicationno, bool create)
        {
            var coa = await _financeServices.GetOrCreateCoaForCandidate(applicationno, create);
            return coa;
        }

        [HttpGet("clbal/{accountid}")]
        public async Task<long> ClBalanceOfAnAccount(int accountid) {
            var bal = await _financeServices.GetClosingBalIncludingSuspense(accountid);
            return bal;
        }
        [HttpPost("coa")]
        public async Task<Coa> AddNewCOA(COAToAddDto coa)
        {
            var obj = await _financeServices.AddNewCOA(coa);
            return obj;
        }

        [HttpPut("coa")]
        public async Task<Coa> EditCOA(Coa coa)
        {
            var c = await _financeServices.EditCOA(coa);
            return c;
        }

        [HttpDelete("coa/{id}")]
        public async Task<bool> DeleteCOA(int id)
        {
            var b = await _financeServices.DeleteCOA(id);
            return b;
        }

        [HttpGet("coabygroup/{accountgroup}")]
        public async Task<ActionResult<ICollection<Coa>>> GetCOAFromAccountGroup(string accountgroup)
        {
            var coas = await _financeServices.GetCOAsForAccountGroup(accountgroup);
            return Ok(coas);
        }
        
        //transactions
        [HttpGet("vouchers")]
        public async Task<Pagination<FinanceVoucher>> GetFinanceTransactions([FromQuery]TransactionParams tParams)
        {
            var trans = await _financeServices.GetFinanceVouchers(tParams);
            return trans;
        }

        [HttpGet("vouchers/{id}")]
        public async Task<FinanceVoucher> GetFinanceTransactionAsync(int id) {
            var trans = await _financeServices.GetFinanceVoucher(id);
            return trans;
        }

        [HttpGet("matchingaccountnames/{testname}")]
        public async Task<ActionResult<ICollection<string>>> GetmatchingAccountNames(string testname)
        {
            var names = await _financeServices.GetMatchingCOANames(testname);
            if(names==null || names.Count==0) return null;
            return Ok(names);

        }

        [HttpDelete("voucher/{id}")]
        public async Task<bool> DeleteFinanceVoucher(int id)
        {
            var b = await _financeServices.DeleteFinanceVoucher(id);
            return b;
        }

        [HttpGet("statementofaccount/{id}/{fromDate}/{uptoDate}")]
        public async Task<ActionResult<StatementOfAccountDto>> GetStatementOfAccount(int id, string fromDate, string uptoDate)
        {
            var isDate1 = DateTime.TryParse(fromDate, out DateTime temp1);
            var isDate2 = DateTime.TryParse(uptoDate, out DateTime temp2);

            if(!isDate1 || !isDate2) return BadRequest(new ApiResponse(404, "invalid Dates"));

            var dto = await _financeServices.GetStatementOfAccount(id, temp1, temp2);

            //if(dto==null) return NotFound(new ApiResponse(404, "Not Found"));

            return Ok(dto);
        }


        [HttpPost("newpaymentfromcandidate")]
        public async Task<ActionResult<ApiReturnDto>> NewPaymentFromCandidate(VoucherToAddNewPayment dto) {
            
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            var returnDto = new ApiReturnDto();

            var dateUploaded = DateTime.Now;
            var entries = new List<VoucherEntry>();
            entries.Add(new VoucherEntry{TransDate = dto.PaymentDate,
                CoaId=dto.DebitCOAId, Dr = dto.Amount, Narration = "" });
            entries.Add(new VoucherEntry{TransDate = dateUploaded,
                CoaId=dto.CreditCOAId, Cr = dto.Amount, Narration = "" });
            
            var vouchertoaddDto = new VoucherToAddDto(0, "R", 0, dto.PaymentDate, dto.CreditCOAId,dto.Amount,dto.Narration,entries, loggedInUser.DisplayName);
            
            var voucherAdded= await _financeServices.AddNewVoucher(vouchertoaddDto, loggedInUser.loggedInEmployeeId);

            if(voucherAdded==null) {
                returnDto.ErrorMessage="Failed to create the voucher";
                return returnDto;
            }
            returnDto.ErrorMessage="";
            returnDto.ReturnInt=voucherAdded.VoucherNo;

            return Ok(returnDto);
        }
        
        [HttpPost("RegisterNewVoucher"), DisableRequestSizeLimit]
          public async Task<ActionResult<ApiReturnDto>> RegisterNewVoucherWithUploads()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               var returnDto = new ApiReturnDto();

               var Attachmentlist = new List<VoucherAttachment>();
               var dateUploaded = DateTime.Now;

               try
               {
                    var modelData = JsonSerializer.Deserialize<VoucherToAddDto>(Request.Form["data"],   //THE Voucher OBJECT
                         new JsonSerializerOptions {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                         });
                    
                    var files = Request.Form.Files;
                    //create the Voucher object first, because the voucherAttaachments need Voucher.VoucherEntry.Id
                    
                    var voucherAdded= await _financeServices.AddNewVoucher(modelData, loggedInUser.loggedInEmployeeId);

                    if(voucherAdded==null) {
                        returnDto.ErrorMessage="Failed to create the voucher";
                        return returnDto;
                    }
                    
                    if(files==null || files.Count==0) return Ok("");

                    //save the uploaded file stream in the designated folder
                    var voucherId = voucherAdded.VoucherNo;
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    var memoryStream = new MemoryStream();

                    foreach (var file in files)
                    {
                         if (file.Length==0) continue;
                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         
                         var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                         if(System.IO.File.Exists(fullPath)) continue;
                         var dbPath = Path.Combine(folderName, fileName); 

                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                            file.CopyTo(stream);
                         }
                        //insert in voucherAttachments
                        var attachment = new VoucherAttachment(voucherId, Convert.ToInt32(file.Length/1024), file.FileName, fullPath, dateUploaded, loggedInUser.loggedInEmployeeId);
                        Attachmentlist.Add(attachment);
                    }

                    if (Attachmentlist.Count > 0) await _financeServices.AddVoucherAttachments(Attachmentlist);
                    returnDto.ReturnInt=voucherId;  //voucherid is voucherNo
                    return Ok(returnDto);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }

        [HttpGet("debitapprovalspending")]
        public async Task<ActionResult<ICollection<PendingDebitApprovalDto>>> GetDebitApprovalsPending()
        {
            var dto = await _financeServices.GetPendingDebitApprovals();
            if(dto==null) return Ok(null);
            return Ok(dto);
        }

        [HttpPut("confirmdebitapprovals")]
        public async Task<bool> ConfirmCashBankDebitEntries(ICollection<UpdatePaymentConfirmationDto> debitDto)
        {
            var success = await _financeServices.UpdateCashAndBankDebitApprovals(debitDto);

            return success;
        }

        [HttpPut("updatevoucherwithfiles"), DisableRequestSizeLimit]
        public async Task<ActionResult> UpdateVoucherWithUpload()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               
               int voucherid=0;

               var voucherAttachmentlist = new List<VoucherAttachment>();

               try
               {
                    var modelData = JsonSerializer.Deserialize<FinanceVoucher>(Request.Form["data"],  
                         new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                    
                    var files = Request.Form.Files;                    
                    var voucherObjectDto = await _financeServices.UpdateFinanceVoucherWithFileUploads(modelData);
                    var voucherObject = voucherObjectDto.FinanceVoucher;
                    var newAttacments = voucherObjectDto.NewAttachments;

                    if(voucherObject==null) return BadRequest(new ApiResponse(404, "Failed to update Finance Voucher object"));
                    voucherid=voucherObject.Id;
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    //var attachmentTypes = modelData.UserAttachments;

                    foreach (var file in files)     //files are new ones uploaded
                    {
                         if(file.Length == 0) continue;
                         /* 
                         1. files uploaded but not present in existing file attachments are the ones to be uploaded, 
                            and hence also to be added in _context.VoucherAttachments Object
                         2. The voucherAttachments collection  could already be having files uploaded earlier, and physical fils in the images folder, 
                            those are to be ignored and not added to the _context.UserAttachments object
                         */
                         //if(modelData.VoucherAttachments.Any(x => x.FileName == file.FileName)) continue;   //no need to upload files that are NOT NEW

                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                         if(System.IO.File.Exists(pathToSave + @"\" + fileName)) continue;
                         
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName);
                         
                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }

                         newAttacments.Add(new VoucherAttachment(modelData.Id,Convert.ToInt32(file.Length/1024),fullPath, 
                            @"'\" + folderName + @"\" + fileName + "'", DateTime.Now, loggedInUser.loggedInEmployeeId ));

                    }

                    if(newAttacments.Count > 0) await _financeServices.AddVoucherAttachments(newAttacments);
                    

                    return Ok();
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }
        
	}
}