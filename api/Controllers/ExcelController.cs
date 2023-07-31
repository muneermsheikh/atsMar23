//using System.Web.Http;

//using Microsoft.AspNetCore.Http.HttpContext;
using System.Net.Http.Headers;
using api.Errors;
using api.Extensions;
using core.Entities.Identity;
using core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    public class ExcelController : BaseApiController
    {
        private readonly IExcelService _excelServices;
        Microsoft.Office.Interop.Excel.Application _objexcel;
        private readonly UserManager<AppUser> _userManager;

        public ExcelController(
            IExcelService excelServices, 
            Microsoft.Office.Interop.Excel.Application objexcel,
            UserManager<AppUser> signManager)
        {
            _excelServices = excelServices;
            _objexcel = objexcel;
            _userManager = signManager;
        }
        
        [HttpPost("convertToDb/{fileNamewithPath}")]
        public async Task<ActionResult<string>> ConvertprospectiveXLtoDB(string FileNameWithPath)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            if (loggedInUser==null) return BadRequest("User email not found");

            var errorString = await _excelServices.ReadAndSaveProspectiveXLToDb(FileNameWithPath,loggedInUser.loggedInEmployeeId,loggedInUser.DisplayName);
            if(errorString=="") return Ok("");
            return BadRequest(new ApiResponse(404, errorString));
        }

        [HttpPost("uploadProspectiveFile"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> UploadProspectiveXLS()
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            if (loggedInUser==null) return BadRequest("User email not found");

            string FileNameWithPath="";
            try
               {
                    var files = Request.Form.Files;
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    var memoryStream = new MemoryStream();

                    foreach (var file in files)
                    {
                         if (file.Length==0) continue;
                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         
                         FileNameWithPath = Path.Combine(pathToSave, fileName);        //physical path
                         if(System.IO.File.Exists(FileNameWithPath)) continue;
                         var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                         using (var stream = new FileStream(FileNameWithPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }
                    }
                    
                    
            }   catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
               
           
            return FileNameWithPath;
        }
         

    }


}