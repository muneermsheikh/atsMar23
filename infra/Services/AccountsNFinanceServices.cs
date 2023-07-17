using core.Dtos;
using core.Dtos.fiance;
using core.Entities.AccountsNFinance;
using core.Entities.Attachments;
using core.Interfaces;
using core.Params;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class AccountsNFinanceServices : IAccountsNFinanceServices
	{
        	private readonly ATSContext _context;
		public AccountsNFinanceServices(ATSContext context)
		{
            _context = context;
		}

	//chart of account
		
		public async Task<ICollection<Coa>> GetCOAList()
		{
			var qry = await _context.COAs.OrderBy(x => x.AccountName).ToListAsync();
			return qry;
		}

		public async Task<ICollection<Coa>> GetCoasForPayments(int applicationno)
		{
			var qry = await _context.Candidates.Where(x => x.ApplicationNo==applicationno).FirstOrDefaultAsync();
			if(qry==null) return null;
			
			var ano=Convert.ToString(applicationno);
			
			var coas = await _context.COAs.Where(x => x.AccountClass.ToLower()=="candidate" 
				&& x.AccountName.Contains(ano) 
				&& x.AccountType.ToLower()=="b")
			.ToListAsync();

			return coas;
		}

		public async Task<ICollection<Coa>> GetCOAsForAccountGroup(string accountgroup) {
			var coas = await _context.COAs
				.Where(x => x.AccountClass.ToLower()== accountgroup.ToLower())
				.OrderBy(x => x.AccountName).ToListAsync();
			return coas;
		}

		public async Task<long> GetClosingBalIncludingSuspense(int accountid) {
			int cl = 0;
			var clBal = await (from entries in _context.VoucherEntries where entries.CoaId==accountid
				group entries by entries.CoaId into g
				select new {clBal= g.Sum(x => -x.Cr + x.Dr)})
				.FirstOrDefaultAsync();
			
			if (clBal!=null) cl = Convert.ToInt32(clBal.clBal);
			
			var coa = await _context.COAs.FindAsync(accountid);

			return cl + coa.OpBalance;

		}
		public async Task<CoaDto> GetOrCreateCoaForCandidate(int applicationno, bool create)
		{
			var candidate = await _context.Candidates.Where(x => x.ApplicationNo==applicationno).FirstOrDefaultAsync();
			if(candidate==null) return null;

			var ano=Convert.ToString(applicationno);
			
			var coadto = await (from c in _context.COAs 
				where c.AccountClass=="Candidate" 
					&& c.AccountName.Contains(ano)
					&& c.AccountType.ToLower()=="b"
					select new CoaDto {
						Id=c.Id,
						AccountClass=c.AccountClass,
						AccountName=c.AccountName,
						AccountType=c.AccountType,
					}
				).SingleOrDefaultAsync();

			if(coadto == null & !create) return null;
			
			if(coadto==null && create) {
				var dto = new COAToAddDto{
					Divn = "R",
					AccountType="B",
					AccountName = candidate.FullName + "- App No." + candidate.ApplicationNo,
					AccountClass="Candidate",
					OpBalance=0
				};
				var coaCreated = await AddNewCOA(dto);
				if (coaCreated==null) return null;
				coadto = new CoaDto {
					Id=coaCreated.Id, Divn=coaCreated.Divn, AccountType=coaCreated.AccountType,
					AccountClass=coaCreated.AccountClass, AccountName=coaCreated.AccountName
				};
			}
			//get cl balance
			var clBal = await GetClosingBalIncludingSuspense(coadto.Id);
			coadto.ClBalance=clBal;

			return coadto;

		}
		
		public async Task<Pagination<Coa>> GetCOAs(CoaParams coaParams)
		{
			var qry = _context.COAs.AsQueryable();
			
			if(!string.IsNullOrEmpty(coaParams.Search)) {
				qry = qry.Where(x => x.AccountName.ToLower().Contains(coaParams.Search.ToLower()) );
			} else {
				if(!string.IsNullOrEmpty(coaParams.AccountName)) qry = qry.Where(x => x.AccountName==coaParams.AccountName);
				if(!string.IsNullOrEmpty(coaParams.Divn)) qry = qry.Where(x => x.Divn==coaParams.Divn);
				if(!string.IsNullOrEmpty(coaParams.AccountType)) qry = qry.Where(x => x.AccountType==coaParams.AccountType);
			}
			
			if(!string.IsNullOrEmpty(coaParams.Sort)) {
				switch(coaParams.Sort.ToLower()) {
					case "name":
						qry = qry.OrderBy(x => x.AccountName);
						break;
					case "namedesc":
						qry = qry.OrderByDescending(x => x.AccountName);
						break;
					case "type":
						qry = qry.OrderBy(x => x.AccountType).ThenBy(x => x.AccountName);
						break;
					case "typedesc":
						qry = qry.OrderByDescending(x => x.AccountType).ThenByDescending(x => x.AccountType);
						break;
					case "divn":
						qry = qry.OrderBy(x => x.Divn).ThenBy(x => x.AccountName);
						break;
					case "divndesc":
						qry = qry.OrderByDescending(x => x.Divn).ThenByDescending(x => x.AccountName);
						break;
					default:
						break;
				}
			}
			var totalCount = await qry.CountAsync();

			var coas = await qry.Skip((coaParams.PageIndex-1)*coaParams.PageSize).Take(coaParams.PageSize).ToListAsync();

			return new Pagination<Coa>(coaParams.PageIndex, coaParams.PageSize, totalCount, coas);
		}


		public async Task<Coa> AddNewCOA(COAToAddDto coa)
		{
			var obj = new Coa(coa.Divn, coa.AccountType, coa.AccountName, coa.AccountClass, coa.OpBalance);
            _context.Add(obj);

			var ct = await _context.SaveChangesAsync();

			if (ct > 0) return obj;
			
			return null;
		}

		public async Task<bool> DeleteCOA(int id)
		{
			var coa = await _context.COAs.FindAsync(id);
			if (coa==null) return false;

            	_context.COAs.Remove(coa);

            	return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Coa> EditCOA(Coa coa)
		{
			var c = await _context.COAs.FindAsync(coa.Id);

			if(c==null) return null;

			_context.Entry(c).CurrentValues.SetValues(coa);

			_context.Entry(c).State=EntityState.Modified;

			if(await _context.SaveChangesAsync() > 0) return coa;
			
			return null;

		}

	//finance transactions
		public async Task<bool> DeleteFinanceVoucher(int id)
		{
			var t = await _context.FinanceVouchers.FindAsync(id);
            
            	if(t==null) return false;

            	_context.FinanceVouchers.Remove(t);

            	return await _context.SaveChangesAsync() > 0;
		}


		public async Task<VoucherWithNewAttachmentDto> UpdateFinanceVoucherWithFileUploads(FinanceVoucher model)
		{
			var fileDirectory = Directory.GetCurrentDirectory();
			List<string>  attachmentsToDelete = new List<string>();          //lsit of files to delete physically from the api space
               	List<VoucherAttachment> attachmentsToAdd = new List<VoucherAttachment>();
			
			var existingVoucher = await _context.FinanceVouchers.Where(x => x.Id == model.Id)
				.Include(x => x.VoucherEntries)
				.Include(x => x.VoucherAttachments)
				.FirstOrDefaultAsync();

            	if(existingVoucher==null) return null;

            	_context.Entry(existingVoucher).CurrentValues.SetValues(model);

			//delete from DB those child items which are not present in the model
			foreach(var existingItem in existingVoucher.VoucherEntries)
			{
				if(!model.VoucherEntries.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
				{
					_context.VoucherEntries.Remove(existingItem);
					_context.Entry(existingItem).State=EntityState.Deleted;
				}
			}
            	
			//items that are not deleted, are either to be updated or new added;
			foreach(var item in model.VoucherEntries)
			{
				var existingItem = existingVoucher.VoucherEntries.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
				if (existingItem != null) {
					_context.Entry(existingItem).CurrentValues.SetValues(item);
					_context.Entry(existingItem).State = EntityState.Modified;
				} else {
					var newItem = new VoucherEntry {
						FinanceVoucherId=existingVoucher.Id,
						TransDate = item.TransDate,
						CoaId = item.CoaId,
						AccountName = item.AccountName,
						Dr = item.Dr,
						Cr = item.Cr,
						Narration = item.Narration
					};
					existingVoucher.VoucherEntries.Add(newItem);
					_context.Entry(newItem).State = EntityState.Added;
				}
			}

			foreach(var existingItem in existingVoucher.VoucherAttachments)
			{
				if(!model.VoucherAttachments.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
				{
					_context.VoucherAttachments.Remove(existingItem);
					_context.Entry(existingItem).State=EntityState.Deleted;
					//prepare to delete files physically from storage folder
					var filepath = existingItem.Url ?? fileDirectory + "/assets/images";
                              attachmentsToDelete.Add(filepath + "/" + existingItem.FileName);        //save file nams to delete later
				}
			}
            	
			//items that are not deleted, are either to be updated or new added;
			foreach(var item in model.VoucherAttachments)
			{
				var existingItem = existingVoucher.VoucherAttachments.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
				if (existingItem != null) {
					_context.Entry(existingItem).CurrentValues.SetValues(item);
					_context.Entry(existingItem).State = EntityState.Modified;
				} 
				/*		//new attachments are inserted in voucherAttachment table after they are uploaded to the designated folder - in the Controller
				else {
					var newItem = new VoucherAttachment (model.Id, 
					item.AttachmentSizeInBytes, item.FileName, item.Url, item.DateUploaded, 0);
					
					existingVoucher.VoucherAttachments.Add(newItem);
					_context.Entry(newItem).State = EntityState.Added;
					attachmentsToAdd.Add(item);
				}
				*/
			}


			_context.Entry(existingVoucher).State=EntityState.Modified;

			int recordsAffected = 0;

			try {
				recordsAffected = await _context.SaveChangesAsync();
			} catch (Exception ex)
			{
				Console.Write(ex.Message);
				return null;
			}
			
			if(recordsAffected > 0 && attachmentsToDelete.Count > 0) {
				do {
					try {
						File.Delete(attachmentsToDelete[attachmentsToDelete.Count]);
					} catch (Exception ex) {
						Console.Write(ex.Message);
					}
				} while (attachmentsToDelete.Count > 0);
			}

			return new VoucherWithNewAttachmentDto {
				FinanceVoucher=existingVoucher,
				NewAttachments=attachmentsToAdd
			};
			

		}

		public async Task<bool>UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> models)
		{
			var existingDBtoUpdate = await _context.VoucherEntries
				.Where(x => models.Select(x => x.VoucherEntryId).ToList().Contains(x.CoaId))
				.AsNoTracking() .ToListAsync();
			
			foreach(var item in models)
			{
				var existingItem = existingDBtoUpdate
					.Where(c => c.Id == item.VoucherEntryId && c.Id != default(int)).SingleOrDefault();
				if(existingItem != null) {
					existingItem.DrEntryApproved=true;
					existingItem.DrEntryApprovedOn=item.DrEntryApprovedOn;
					existingItem.DrEntryApprovedByEmployeeById=item.DrEntryApprovedByEmployeeById;
					_context.Entry(existingItem).State = EntityState.Modified;
				}
			}

			return await _context.SaveChangesAsync() > 0;
		}

	//vouchers
		public async Task<FinanceVoucher> AddNewVoucher(VoucherToAddDto dto, int loggedInEmployeeId)
		{
			var accountname=await GetAccountNameFromCOA(dto.CoaId);
			if(string.IsNullOrEmpty(accountname)) return null;

			dto.VoucherNo=await GetNextVoucherNo();
			foreach(var item in dto.VoucherEntries) {
				if(string.IsNullOrEmpty(item.AccountName)) {
					item.AccountName = await GetAccountNameFromCOA(item.CoaId);
				}
			}

			var t = new FinanceVoucher(dto.Divn, dto.VoucherNo, dto.VoucherDated, dto.CoaId, accountname, dto.Amount, loggedInEmployeeId, dto.Narration, dto.VoucherEntries);

            	_context.FinanceVouchers.Add(t);

            	if (await _context.SaveChangesAsync() > 0) return t;

            	return null;
		}

		public async Task<Pagination<FinanceVoucher>> GetFinanceVouchers(TransactionParams tParams)
		{
			var qry = _context.FinanceVouchers.AsQueryable();

			if(!string.IsNullOrEmpty(tParams.Divn)) qry = qry.Where(x => x.Divn==tParams.Divn);

			var totalCount = await qry.CountAsync();
			
			if(totalCount==0) return null;

			bool modified=false;

			var trans = await qry.OrderBy(x => x.VoucherNo)
				.Select(x => new FinanceVoucher {
					Id=x.Id, VoucherNo=x.VoucherNo, VoucherDated=x.VoucherDated, CoaId=x.CoaId, AccountName=x.AccountName, 
					Amount=x.Amount, Narration=x.Narration})
				.Skip((tParams.PageIndex-1)*tParams.PageSize).Take(tParams.PageSize)
				.ToListAsync();
			
			foreach(var item in trans) {
				if(string.IsNullOrEmpty(item.AccountName)) {
					var s = await GetAccountNameFromCOA(item.CoaId);
				item.AccountName=s;
				_context.Entry(item).State=EntityState.Modified;
				modified=true;
			}}

			if(modified) await _context.SaveChangesAsync();

			var pages =  new Pagination<FinanceVoucher>(tParams.PageIndex, tParams.PageSize, totalCount,trans);

			/*
			//op and cl balances
			qry = qry.Where(x => x.Approved);
			var totalSum =  await qry.SumAsync(x => x.Amount);
			var op = await qry.Where(x => x.Approved==true && x.TransDate < tParams.DateFrom).SumAsync(x => x.Amount);
			var cl = op + totalSum;

			var dto = new FinanceTransactionHeaderDto(tParams.Divn, tParams.DateFrom, tParams.DateUpto, op, cl, pages);

			return dto;
			*/

			return pages;
		}

		private async Task<string> GetAccountNameFromCOA(int coaid) {
			var s = await _context.COAs.Where(x => x.Id==coaid).Select(x=> x.AccountName).FirstOrDefaultAsync();
			if(s==null) return "";
			return s;
		}
		public async Task<FinanceVoucher> GetFinanceVoucher(int id) {
			
			var trans = await _context.FinanceVouchers
				.Where(x => x.Id == id)
				.Include(x => x.VoucherEntries)
				//.Include(x => x.VoucherAttachments)
				.FirstOrDefaultAsync();
			
			var attachs = await _context.VoucherAttachments.Where(x => x.FinanceVoucherId==id).ToListAsync();

			trans.VoucherAttachments=attachs;

			return trans;
		}

		public async Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateTime fromDate, DateTime UptoDate)
		{
			DateTime uptoDate = UptoDate.Hour < 1 ? UptoDate.AddHours(23) : UptoDate;
			
			var trans =  await (from i in _context.VoucherEntries where i.CoaId == accountid && i.TransDate >= fromDate && i.TransDate <= uptoDate
				join v in _context.FinanceVouchers on i.FinanceVoucherId equals v.Id
				join a in _context.COAs on i.CoaId equals a.Id
				orderby i.TransDate descending
				select new StatementOfAccountItemDto {
					VoucherNo = v.VoucherNo,
					TransDate = i.TransDate,
					CoaId = a.Id,
					AccountName = a.AccountName,
					Dr = i.Dr,
					Cr = i.Cr,
					Narration = i.Narration
				}).ToListAsync();
						
						
			var transtest = await (from v in _context.VoucherEntries where v.CoaId==accountid 
				select new {v.Id, v.TransDate, v.CoaId, v.AccountName, v.Dr, v.Cr}).OrderByDescending(x => x.TransDate).ToListAsync();
			var opBal = await (from v in _context.VoucherEntries where v.CoaId==accountid && v.TransDate < fromDate
				group v by v.CoaId into g 
				select new {Id = g.Key, Bal = g.Sum(e => -e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();
			var oclBalTest = await (from v in _context.VoucherEntries where v.CoaId==accountid && v.TransDate >= uptoDate
				select new {v.Id, v.TransDate, v.CoaId, v.AccountName, v.Dr, v.Cr}).ToListAsync();

			var BalForThePeriod = await (from v in _context.VoucherEntries 
					where v.CoaId==accountid 
						&& v.TransDate >= fromDate 
						&& v.TransDate <= uptoDate
				group v by v.CoaId into g 
				select new {Id = g.Key, Bal = -g.Sum(e => e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();

			var dto = new StatementOfAccountDto{
				AccountId=accountid,
				AccountName= trans.Count()==0 ? await GetAccountNameFromCOA(accountid) : trans[0].AccountName, 
				FromDate = fromDate,
				UptoDate = uptoDate,
				StatementOfAccountItems = trans,
				OpBalance = opBal==null? 0 : opBal.Bal,
				ClBalance = BalForThePeriod==null ? 0 : BalForThePeriod.Bal
			};

			return dto;	
		}

		public async Task<ICollection<PendingDebitApprovalDto>> GetPendingDebitApprovals()
		{
			var cashandbank = await _context.COAs.Where(x => x.AccountClass=="CashAndBank").Select(x => x.Id).ToListAsync();

			var qry = await (from e in _context.VoucherEntries 
				where e.DrEntryApproved != true & e.Dr > 0 & cashandbank.Contains(e.CoaId)
				join v in _context.FinanceVouchers on e.FinanceVoucherId equals v.Id
				select new PendingDebitApprovalDto(e.Id, v.VoucherNo, v.VoucherDated, e.CoaId,
					 e.AccountName, e.Dr)).ToListAsync();
			
			return qry;
		}

		public async Task<ICollection<string>> GetMatchingCOANames(string testName)
		{
			var existingnames = await _context.COAs
				.Where(x => x.AccountName.ToLower().Contains(testName.ToLower()))
				.Select(x => x.AccountName)
				.ToListAsync();
			
			return existingnames;

		}

		public async Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> voucherattachments)
		{
			foreach(var item in voucherattachments) {
				_context.VoucherAttachments.Add(item);
			}
			
			try {
				await _context.SaveChangesAsync();
			} catch (System.Exception ex ) {
				return ex.Message;
			}
			return "";
		}

		private async Task<int> GetNextVoucherNo() {
			var nextno = await _context.FinanceVouchers.MaxAsync(x => x.VoucherNo);
			return nextno==0 ? 1000 : nextno + 1;

		}


	}
}