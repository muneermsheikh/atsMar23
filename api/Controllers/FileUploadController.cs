using System.Net.Http.Headers;
using core.Entities.Attachments;
using core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using infra.Data;


using api.Extensions;
using core.Entities.Users;
using System.Text.Json;
using core.Interfaces;
using api.Errors;

namespace api.Controllers
{
     public class FileUploadController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        const string FILE_PATH = @"D:\UploadedFiles\";
        private readonly ATSContext _context;
        private readonly IUserService _userService;
        private readonly IProspectiveCandidateService _prospectiveService;
        private readonly IUserGetAndUpdateService _userGetAndUpdateService;
        private readonly IExcelService _excelService;
                
        public FileUploadController(
            ATSContext context, 
            UserManager<AppUser> userManager, 
            IWebHostEnvironment env, 
            IUserService userService, 
            IUserGetAndUpdateService userGetAndUpdateService,
            IProspectiveCandidateService prospectiveService, 
            IExcelService excelService)
        {
            _excelService = excelService;
            _userGetAndUpdateService = userGetAndUpdateService;
            _prospectiveService = prospectiveService;
            _userService = userService;
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("downloadfile/{attachmentid:int}")]
        public async Task<ActionResult> DownloadFile(int attachmentid)
        {
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var attachment = await _context.UserAttachments.FindAsync(attachmentid);
            if (attachment==null) return NotFound("the requested record does not exist");

            var FileName=attachment.url;
            if(string.IsNullOrEmpty(FileName)) return BadRequest("No URL found in the attachment record");

            if(!System.IO.File.Exists(FileName)) return NotFound("the File " + attachment.FileName + " does not exist");

            //var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

        
        [Authorize]
        [HttpGet("downloadprospectivefile/{prospectiveid:int}")]
        public async Task<ActionResult> DownloadProspectiveFile(int prospectiveid)
        {
            
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

        [HttpPost("prospectiveorapplications"), DisableRequestSizeLimit]
        public async Task<IActionResult> ConvertProspectiveData()
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            if (loggedInUser==null) return BadRequest("User not logged in");
                
            //check for uploaded files
            var files = Request.Form.Files;

            string ErrorString="";
            try{
                if(files.Count() > 0) {
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        if(FileExtn != ".xlsx") continue;

                        var filename=file.FileName.Substring(0,9).ToLower();

                        if(filename != "prospecti" && filename !="applicati") continue;

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) continue;

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        
                        if(filename=="prospecti") {
                            ErrorString = await _excelService.ReadAndSaveProspectiveXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        } else {
                            ErrorString = await _excelService.ReadAndSaveApplicationsXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        }
                    }
                }    

            } catch (Exception ex) {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) return BadRequest("Failed to read and update the prospective file");
            return Ok();
        }
        
        //save candidate attachments.
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> RegisterNewCandidateWithAttachments()
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            if (loggedInUser==null) return BadRequest("User not logged in");
            
            int candidateid=0;
            int applicationno=0;

            string attachmenttype="";
            var userattachmentlist = new List<UserAttachment>();
            var reqst = Request.Form["data"];
            
            try
            {
                if(reqst.Count > 0) {
                    var modelData = JsonSerializer.Deserialize<Candidate>
                        (Request.Form["data"],  
                            new JsonSerializerOptions {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    //Console.Write(modelData);
                    if(modelData != null) {
                        var candidateObject = await _userGetAndUpdateService.UpdateCandidateAsync(modelData);
                        if(candidateObject==null) return BadRequest(new ApiResponse(404, "Failed to update candidate object"));
                        candidateid=candidateObject.Candidate.Id;
                        applicationno=candidateObject.Candidate.ApplicationNo;
                    }
                }
                
                //now check for uploaded files
                var files = Request.Form.Files;
                if(files.Count() > 0) {
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        
                        if(!fileName.Contains(applicationno.ToString().Trim())) fileName = applicationno.ToString().Trim() + "-" + fileName;
                        //remove trailing '!!' + this.candidateId.toString()
                        var fileNameParts = fileName.Split("!!");       //characters after this are the candidate id
                        if(fileNameParts.Length > 1) {
                            fileName = fileNameParts[0];
                            attachmenttype=fileNameParts[2];
                        } else {
                            fileName=fileNameParts[0];
                            attachmenttype="CV";
                        }
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) continue;
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        var userattachment = new UserAttachment(loggedInUser.Id, candidateid, fileName, attachmenttype, file.Length/1048, 
                            dbPath, loggedInUser.loggedInEmployeeId, DateTime.UtcNow );
                        userattachmentlist.Add(userattachment);
                    }
                    
                    var attachmentsUpdated = await _userService.AddUserAttachments(userattachmentlist);
                }

                return Ok("Candidate Updated.");

            }  catch (Exception ex) {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
        }

        //[Authorize]
        //[HttpPost("UploadAndSaveXLToDb")]
       
        private bool IsNumeric(string input) {
            int test;
            return int.TryParse(input, out test);
        }
        private string GetConnectionString(string XLFileName)
        {
            
            string connString = "";
            string strFileType = Path.GetExtension(XLFileName).ToLower();
            
            if (strFileType.Trim() == ".xls")
            {
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + XLFileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (strFileType.Trim() == ".xlsx")
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + XLFileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
        
            return connString;
        }


       [HttpGet("LoadDocument")]
        public LoadedDocument LoadDocument()
        {
            string documentName = "invoice.docx";
            LoadedDocument document = new LoadedDocument()
            {
            DocumentData = Convert.ToBase64String(
                System.IO.File.ReadAllBytes("App_Data/" + documentName)),
            DocumentName = documentName
            };

            return document;
        }


    }
}