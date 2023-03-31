using System.Net.Http.Headers;
using System.Text.Json;
using api.Extensions;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
     [Authorize]
     public class UploadController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;
          private readonly ATSContext _context;
          private readonly IUserService _userService;
          public UploadController(UserManager<AppUser> userManager, ATSContext context, IUserService userService  )
          {
               _context = context;
               _userManager = userManager;
               _userService = userService;
          }

     /*
          [HttpPost, DisableRequestSizeLimit]
          public async Task<IActionResult> Upload()
          {
               try
               {
                    var formCollection = await Request.ReadFormAsync();
                    //var file = formCollection.Files.First();
                    //var file = Request.Form.Files[0];
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    foreach(var file in formCollection.Files)
                    {
                         if (file.Length > 0)
                         {
                              var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                              var fullPath = Path.Combine(pathToSave, fileName);
                              var dbPath = Path.Combine(folderName, fileName);
                              using (var stream = new FileStream(fullPath, FileMode.Create))
                              {
                                   file.CopyTo(stream);
                              }
                         } 
                         return Ok(new { dbPath });
                    }
               }
               catch (Exception ex)
               {
                    return StatusCode(500, $"Internal server error: {ex}");
               }
          }
     */
          private bool IsAPhotoFile(string fileName)
          {
               return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
          }         
          
          private bool IsDocument(string fileName)
          {
               return fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
          }

          [HttpPost, DisableRequestSizeLimit]
          public async Task<IActionResult> Upload()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser==null) return BadRequest("User not logged in");

               int candidateId=0;
               string attachmenttype="";
               var userattachmentlist = new List<UserAttachment>();
               try
               {
                    //var modelData = JsonConvert.DeserializeObject<Candidate>(Request.Form["data"]); 
                    var modelData = JsonSerializer.Deserialize<Candidate>(Request.Form["data"]);
                    //data is the key that is being passed from client side
                    //file in params will have the posted file
                    
                    var files = Request.Form.Files;
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    if (files.Any(f => f.Length == 0))
                    {
                         return BadRequest();
                    }

                    foreach (var file in files)
                    {
                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         //remove trailing '!!' + this.candidateId.toString()
                         var fileNameParts = fileName.Split("!!");       //characters after this are the candidate id
                         if(fileNameParts.Length >0) {
                              fileName = fileNameParts[0];
                              candidateId = Convert.ToInt32(fileNameParts[1]);
                              attachmenttype=fileNameParts[2];

                         }
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }
                         
                         var userattachment = new UserAttachment();        
                         userattachment.url=dbPath;
                         userattachment.CandidateId=candidateId;
                         userattachment.AttachmentType=attachmenttype;
                         userattachment.DateUploaded=DateTime.UtcNow;
                         userattachment.AttachmentSizeInBytes=file.Length;
                         userattachment.UploadedByEmployeeId=loggedInUser.loggedInEmployeeId;
                         userattachmentlist.Add(userattachment);
                    }
                    foreach(var a in userattachmentlist) {await _context.UserAttachments.AddAsync(a);}
                    await _context.SaveChangesAsync();

                    return Ok("All the files are successfully uploaded.");
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }

          [HttpGet("attachments/{candidateId}")]
          public async Task<IActionResult> GetAllUsers(int candidateId)
          {
               try
               {
                    var users = await _context.UserAttachments.Where(X => X.CandidateId == candidateId).ToListAsync();

                    return Ok(users);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, $"Internal server error: {ex}");
               }
          }

          [HttpGet, DisableRequestSizeLimit]
          [Route("download/{attachmentid}")]
          public async Task<IActionResult> Download(int attachmentid) // [FromQuery] string fileUrl)
          {
               var FileName= await _userService.GetFileUrl(attachmentid);

               if(string.IsNullOrEmpty(FileName)) return BadRequest("Invalid file id, or the file does not exist");

               //var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
               //if (!System.IO.File.Exists(filePath)) return NotFound();
               
               var memory = new MemoryStream();
               await using (var stream = new FileStream(FileName, FileMode.Open))
               {
                    await stream.CopyToAsync(memory);
               }
               memory.Position = 0;
               return File(memory, GetContentType(FileName), FileName);
          }

          private string GetContentType(string path)
          {
               var provider = new FileExtensionContentTypeProvider();
               string contentType;
                         
               if (!provider.TryGetContentType(path, out contentType))
               {
                    contentType = "application/octet-stream";
               }
                    
               return contentType;
          }
     }
}