using api.Extensions;
using core.Entities.Identity;
using core.Entities.Users;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
     public class AttachmentController : BaseApiController
     {

          private readonly ATSContext _context;
          private readonly UserManager<AppUser> _usermanager;
          public AttachmentController(ATSContext context, UserManager<AppUser> usermanager)
          {
               _usermanager = usermanager;
               _context = context;
          }

          [HttpGet("attachments/{candidateid}")]
          public async Task<IActionResult> GetCandidateAttachments(int candidateid)
          {
               try
               {
                    var attachments = await _context.UserAttachments.Where(x => x.CandidateId == candidateid).ToListAsync();

                    return Ok(attachments);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, $"Internal server error: {ex}");
               }
          }

          [HttpPost]
          public async Task<IActionResult> CreateAttachment([FromBody] UserAttachment user)
          {
               var loggedInUser = await _usermanager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser == null) return Unauthorized("Access allowed to authorized loggin user only");

               try
               {
                    if (user is null)
                    {
                         return BadRequest("User object is null");
                    }

                    if (!ModelState.IsValid)
                    {
                         return BadRequest("Invalid model object");
                    }

                    //user.Id = Guid.NewGuid();
                    user.UploadedByEmployeeId=loggedInUser.loggedInEmployeeId;
                    user.DateUploaded = DateTime.UtcNow;
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    return StatusCode(201);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, $"Internal server error: {ex}");
               }
          }
     }
}