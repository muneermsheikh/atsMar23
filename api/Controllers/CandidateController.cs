using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Dtos;
using core.Entities.Attachments;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;

namespace api.Controllers
{
     //[Authorize]
     public class CandidateController : BaseApiController
     {
          //private readonly IUnitOfWork _unitOfWork;
          static DateTime TimeLast;
          private readonly IMapper _mapper;
          private readonly UserManager<AppUser> _userManager;
          private readonly SignInManager<AppUser> _signInManager;
          private readonly IUserService _userService;
          //private readonly IGenericRepository<Candidate> _candRepo;
          private readonly IWebHostEnvironment _environment;
          private readonly IEmployeeService _empService;
          private readonly IUserGetAndUpdateService _userGetAndUpdateService;
          private readonly IWebHostEnvironment _host;
          const string FILE_PATH = @"D:\UploadedFiles\";
          public CandidateController(
               //IGenericRepository<Candidate> candRepo, 
               IMapper mapper, 
               UserManager<AppUser> userManager, 
               SignInManager<AppUser> signInManager, 
               IWebHostEnvironment host,
               IUnitOfWork unitOfWork, 
               IEmployeeService empService, 
               IWebHostEnvironment environment,
               IUserService userService,
               IUserGetAndUpdateService userGetAndUpdateService)
          {
               _environment = environment;
               //_candRepo = candRepo;
               _userService = userService;
               _signInManager = signInManager;
               _userManager = userManager;
               _mapper = mapper;
               //_unitOfWork = unitOfWork;
               _empService = empService;
               _host = host;
               _userGetAndUpdateService = userGetAndUpdateService;
               CandidateController.TimeLast=new DateTime();
          }

     
          [HttpGet("candidatepages")]
          public async Task<ActionResult<Pagination<CandidateBriefDto>>> GetCandidatePagesAsync([FromQuery]CandidateSpecParams candidateParam)
          {
               var pages = await _userGetAndUpdateService.GetCandidateBriefPaginated(candidateParam);
               if(pages==null) return BadRequest(new ApiResponse(404, "No records found"));
               return Ok(pages);
          }

          
          [Authorize]
          [HttpGet("candidatelist")]
          public async Task<ActionResult<ICollection<CandidateBriefDto>>> GetCandidateListAsync(CandidateSpecParams candidateParam)
          {
               var list = await _userGetAndUpdateService.GetCandidateListBrief(candidateParam);
               
               return Ok(list);
          }

          [HttpGet("byid/{id}")]
          public async Task<ActionResult<Candidate>> GetCandidateById(int id)
          {
               var cand = await _userGetAndUpdateService.GetCandidateByIdWithAllIncludes(id);
               return Ok(cand);
          }

          [HttpGet("byappno/{appno}")]
          public async Task<ActionResult<CandidateBriefDto>> GetCandidateFromApplicationNo(int appno) {
               var cv = await _userGetAndUpdateService.GetCandidateByAppNo(appno);
               if (cv==null) return NotFound(new ApiResponse(404, "Application No. " + appno + " not found"));

               return Ok(cv);
          }
          
          [HttpGet("briefbyid/{id}")]
          public async Task<ActionResult<CandidateBriefDto>> GetCandidateBriefById(int id) {
               var cv = await  _userGetAndUpdateService.GetCandidateBriefById(id);
               
               if (cv==null) return NotFound(new ApiResponse(404, "Candidate with id " + id + " not found"));

               return Ok(cv);
          }
          
          /*
          [HttpGet("candidatebyid/{userid}")]
          public async Task<ActionResult<Candidate>> GetCandidatebyUserId(int userid)
          {
               return await _userService.GetCandidateByIdAsync(userid);
          }

          [HttpGet("candidatebyappuserid/{appuserid}")]
          public async Task<ActionResult<Candidate>> GetCandidateByAppUserid(int appUserId)
          {
          var cands = await _userService.GetCandidateBySpecsIdentityIdAsync(appUserId);
          if (cands == null) return NotFound(new ApiResponse(404));
          return Ok(cands);
          }
          */
          
          [HttpGet("emailexists")]
          public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }

          [HttpGet("cities")]
          public async Task<ActionResult<ICollection<CandidateCity>>> GetCandidateCities()
          {
               var c = await _userService.GetCandidateCityNames();

               if (c.Count() == 0) return NotFound();
               return Ok(c);
          }
          
          
          /*
          [Authorize(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPost]
          public async Task<ActionResult<bool>> UploadUserAttachmentsR(FileToUpload file) {

               var filePathName = FILE_PATH + Path.GetFileNameWithoutExtension(file.FileName) + "-" +
                    DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                    Path.GetExtension(file.FileName);
               
               return Ok(true);

          }
          */

          [Authorize]    //(Roles ="Admin, HRManager, HRSupervisor, HRExecutive")]
          [HttpPost("attachment/{candidateAppUserId}")]
          public async Task<ActionResult<bool>> UploadUserAttachments(ICollection<IFormFile> files, int candidateAppUserId)
          {
               if (files.Count() == 0) return BadRequest(new ApiResponse(404, "No files to attach"));
          
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               int loggedInEmployeeId = loggedInUser == null ? 0 : await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);

               string Errorstring = "";
               var filesUploaded = new List<FileUpload>();
               Errorstring = FileExtensionsOk(files);
               
               if (!string.IsNullOrEmpty(Errorstring))                     
                    return BadRequest(new ApiResponse(402, "file upload failed due to following error: " +
                         Environment.NewLine + Errorstring));

               try
               {
                    foreach (var file in files)
                    {
                         var folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files");
                         if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                         var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", file.FileName);
                         if (!System.IO.File.Exists(path))
                         {
                              using(var stream = new FileStream(path, FileMode.Create))
                              {
                                   await file.CopyToAsync(stream);         //file coied to path
                              }
                              
                              var f = new FileUpload(candidateAppUserId, file.FileName.Substring(0,3),
                                   file.Length, User.GetUsername(), loggedInUser == null ? 0 : loggedInEmployeeId,
                                   DateTime.Now, true);
                              //_unitOfWork.Repository<FileUpload>().Add(f);
                              filesUploaded.Add(f);
                         } else {
                              Errorstring +="file " + file.FileName + " already is uploaded";
                         }
                    }

                    if (filesUploaded.Count > 0) {
                         //await _unitOfWork.Complete();
                         Errorstring = await _userGetAndUpdateService.SaveUploadedFiles(filesUploaded);
                         
                         if(!string.IsNullOrEmpty(Errorstring)) {
                              return Ok(Errorstring + Environment.NewLine + "Other files uploaded successfully");
                         }  else {
                              return Ok();
                         }
                    } 
                    else {
                         return BadRequest(new ApiResponse(404,  "failed to upload" + Environment.NewLine + Errorstring));
                    }
               }
               catch (Exception ex)
               {
                    return BadRequest(new ApiResponse(402, ex.Message));
               }

          }

          [Authorize]    //(Roles ="Admin, HRManager, HRSupervisor, HRExecutive, Candidate")]
          [HttpDelete("deleteUploadedFile")]
          public async Task<ActionResult<bool>> DeleteUploadedFile(FileUpload fileupload)
          {
               var str = await _userGetAndUpdateService.DeleteUploadedFile(fileupload);

               if(string.IsNullOrEmpty(str)) {
                    return Ok(true);
               } else {
                    return BadRequest(new ApiResponse(404, str));        
               }
          }

          /*
          [HttpGet("downloadfile/{id:int}")]
          public async Task<ActionResult> DownloadFile(int id)
          {
               // ... code for validation and get the file
               
               var file = await _unitOfWork.Repository<FileUpload>().GetByIdAsync(id);
               if (file==null) return BadRequest(new ApiResponse(404, "No such file exists"));

               var filePath = Path.Combine(file.UploadedLocation + "\\" + file.Name);  
               // Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", file.Name);
               
               if (!System.IO.File.Exists(filePath)) return BadRequest(new ApiResponse(404, "Though the file exists in database record, no physical file could be located"));

               var provider = new FileExtensionContentTypeProvider();
               if (!provider.TryGetContentType(filePath, out var contentType))
               {
                    contentType = "application/octet-stream";
               }
               
               var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
               return File(bytes, contentType, Path.GetFileName(filePath));
          }
          */

          private string FileExtensionsOk(ICollection<IFormFile> formFiles)
          {
               var ok = true;
               string ext = "";
               string errorString="";
               var files = formFiles.Select(x => new {x.FileName, x.Length}).ToList();
               foreach(var file in files)
               {
                    var fileType = file.FileName.Trim().ToLower().Substring(0,3);
                    switch(fileType)
                    {
                         case "cv-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".docx" || ext == ".doc" || ext == ".pdf");
                              if (!ok) errorString += Environment.NewLine + "only docx, doc or pdf extensions acceptable for attachment type CV";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         case "ec-":
                         case "qc-":
                         case "pp-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".pdf");
                              if (!ok) errorString += Environment.NewLine + "only jpeg, jpg, png or pdf extensions acceptable for attachment type Certificates";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         case "ph-":
                              ext = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                              ok =  (ext == ".jpeg" || ext == ".jpg" || ext == ".png");
                              if (!ok) errorString += Environment.NewLine + "only jpeg, jpg or png extensions acceptable for images";
                              if(file.Length == 0 ||file.Length > 1024*1024*3) errorString +=Environment.NewLine + "File size cannot exceed 3MB";
                              break;
                         default:
                              errorString += "prefix " + fileType + " not recognized";
                              break;
                    }
               }
               return errorString;
          }

          [Authorize]    //(Roles ="Admin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpPut("edituserprof")]
          public async Task<ActionResult<UserAndProfessions>> EditUserProfessions(UserAndProfessions userProfessions)
          {
               var professions = await _userService.EditUserProfessions(userProfessions);
               if (professions == null) return BadRequest(new ApiResponse(404, "Failed to edit the user professions"));
               var profs = _mapper.Map<List<UserProfession>, List<Prof>>(professions.ToList());
               
               return Ok(new UserAndProfessions{CandidateId = userProfessions.CandidateId, CandidateProfessions = profs});
          }
     
          [Authorize]    //(Roles ="Admin, HRManager, HRSupervisor, HRTrainee")]
          [HttpPut]
          public async Task<ActionResult<Candidate>> EditCandidate([FromBody]Candidate candidate)
          {
               var cand = await _userGetAndUpdateService.UpdateCandidateAsync(candidate);
               if (cand == null) return BadRequest(new ApiResponse(404, "Failed to update the candidate"));
               return Ok(cand);
          }

          [HttpGet("downloadfile/{attachmentid:int}")]
          public async Task<ActionResult> DownloadFile(int attachmentid)
          {
               var FileName= await _userService.GetFileUrl(attachmentid);

               if(string.IsNullOrEmpty(FileName)) return BadRequest("Invalid file id, or the file does not exist");

               var memory = new MemoryStream();

               HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
               using (var stream = new FileStream(FileName, FileMode.Open))
               {
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = FileName;   // Path.GetFileName(path);
                    //result.Content.Headers.ContentType = "application/octet-stream"; // new MediaTypeHeaderValue(FileHandler.GetContentType(path)); // Text file
                    result.Content.Headers.ContentLength = stream.Length;

                    return Ok(result);
               }
          }

          
          [HttpPut("updatecandidate"), DisableRequestSizeLimit]
          public async Task<ActionResult> EditCandidateWithUpload()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser==null) return BadRequest("User not logged in");
               
               int candidateid=0;
               string applicationno="";

               var userattachmentlist = new List<UserAttachment>();

               try
               {
                    var modelData = JsonSerializer.Deserialize<Candidate>(Request.Form["data"],  
                         new JsonSerializerOptions {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                    
                    //var modelData = Request.Form["data"];
                         
                    var candidateObjectDto = await _userGetAndUpdateService.UpdateCandidateAsync(modelData);
                    var candidateObject = candidateObjectDto.Candidate;
                    var newAttacments = candidateObjectDto.NewAttachments;

                    if(candidateObject==null) return BadRequest(new ApiResponse(404, "Failed to update candidate object"));
                    candidateid=candidateObject.Id;
                    applicationno=candidateObject.ApplicationNo.ToString().Trim();
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    //var attachmentTypes = modelData.UserAttachments;
                    var files = Request.Form.Files;
                    foreach (var file in files)
                    {
                         if(file.Length == 0) continue;
                         //files uploaded but not prsent in existing file attachments are the new files to be uploaded, and hence also to be added in USERaTTACHMENTS
                         //The userAttachments could already be having files uploaded earlier, and existing in the images folder, those are to be 
                         //ignored and not added to the _context.UserAttachments object
                         if(newAttacments.Where(x => x.FileName != file.FileName).FirstOrDefault() == null) continue;   //no need to upload files that are NOT NEW

                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         fileName = applicationno + "-" + fileName;
                         if(System.IO.File.Exists(pathToSave + @"\" + fileName)) continue;
                         
                         //the filename syntax is: application No + "-" + filename
                         if(!fileName.Contains(applicationno.ToString().Trim())) fileName = applicationno.ToString().Trim() + "-" + fileName;
                         
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName);
                         
                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }

                    }
                    
                    //var attachmentsUpdated = await _userService.AddUserAttachments(userattachmentlist);
                    //candidateObject.UserAttachments=attachmentsUpdated;

                    return Ok("Candidate Updated.");
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }
        
          [HttpPut("updateimageurl"), DisableRequestSizeLimit]
          public async Task<ActionResult> UpdateImageUrl()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               try
               {
                    var file = Request.Form.Files[0];
                    if(file.Length == 0) return BadRequest(new ApiResponse(404, "File not selected"));

                    var candidateidDto = JsonSerializer.Deserialize<CandidateIdDto>(Request.Form["candidateid"],  
                         new JsonSerializerOptions {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                         });

                    var candidateid=candidateidDto.CandidateId;
                    var applicationno=candidateidDto.ApplictionNo.ToString().Trim();
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    if (System.IO.File.Exists(Path.Combine(pathToSave, applicationno, "-", fileName))) return BadRequest(new ApiResponse(404, "File Already Exists"));
                    
                    fileName = applicationno + "-" + fileName;
                    
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                         file.CopyTo(stream);
                    }

                    await _userService.UpdateCandidatePhoto(candidateid, @"\assets\images\" + fileName);

                    return Ok("Image uploaded.");
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }


    }


}