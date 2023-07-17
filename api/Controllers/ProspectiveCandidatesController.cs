using api.Errors;
using api.Extensions;
using core.Dtos;
using core.Entities.Identity;
using core.Interfaces;
using core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class ProspectiveCandidatesController : BaseApiController
     {
          private readonly IProspectiveCandidateService _prospectiveService;
          //private readonly RoleManager<AppRole> _roleManager;
          private readonly ITokenService _tokenService;
          private readonly IUserService _userService;
          private readonly UserManager<AppUser> _userManager;
          
          public ProspectiveCandidatesController(
               UserManager<AppUser> userManager
               , IProspectiveCandidateService prospectiveService
               , IUserService userService
               , ITokenService tokenService
               //, RoleManager<AppRole> roleManager
               )
          {
               _userManager = userManager;
               _userService = userService;
               _tokenService = tokenService;
               //_roleManager = roleManager;
               _prospectiveService = prospectiveService;
          }

          [Authorize]
          [HttpGet]
          public async Task<ActionResult<Pagination<ProspectiveCandidateEditDto>>> GetProspectiveCandidates([FromQuery]ProspectiveCandidateParams pParams)
          {
              var prospectives = await _prospectiveService.GetProspectiveCandidates(pParams);
              if (prospectives == null) return NotFound(new ApiResponse(404, "Not Found"));
              return Ok(prospectives);
          }

          [HttpGet("summary")]
          public async Task<ActionResult<ICollection<ProspectiveSummaryDto>>> GetProspectivesummary([FromQuery]ProspectiveSummaryParams sParams)
          {
               var summary = await _prospectiveService.GetProspectiveSummary(sParams);

               if (summary != null) return Ok(summary);

               return NotFound(new ApiResponse(404, "No data returned"));
          }

          [Authorize]    //(Roles="CreateCV")]
          [HttpPost]
          public async Task<ActionResult<UserDto>> CreateCandidateFromProspectiveModel (ProspectiveCandidateAddDto prospectiveAddDto)
          {
               //if(await _userManager.FindByEmailAsync(prospectiveAddDto.Email) != null ) return BadRequest("cannot create new candidate record, as the email already exists");

               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               prospectiveAddDto.LoggedInUserId = loggedInUser.loggedInEmployeeId;

               var UserReturned = await _prospectiveService.ConvertProspectiveToCandidate(prospectiveAddDto);

               if (UserReturned == null) return BadRequest(new ApiResponse(402, "Failed to create Candidate record from the propective details"));

               return Ok(UserReturned);
          
          }

          [Authorize]
          [HttpGet("headers")]
          public async Task<ActionResult<Pagination<UserHistoryHeaderDto>>> GetCallRecordHeaders([FromQuery]UserHistoryHeaderParams hParams)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               //if (loggedInUser == null) return Unauthorized(new ApiResponse(401, "Unuthorized"));
               var loggedInUserId = loggedInUser.loggedInEmployeeId;

               var headers = await _prospectiveService.GetCallRecordHeaders(hParams);
               if (headers == null) return NotFound(new ApiResponse(404, "No matching call records found"));
               return Ok(headers);
          }

          [Authorize]
          [HttpPut("prospectivelistedit")]
          public async Task<ActionResult> UpdateProductiveCandidateStatus(ICollection<ProspectiveUpdateDto> dto )
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               var UserDto = new LoggedInUserDto{LoggedIAppUsername=loggedInUser.DisplayName, LoggedInAppUserEmail=loggedInUser.Email,LoggedInEmployeeId=loggedInUser.loggedInEmployeeId};
               var ErrorString = await _prospectiveService.EditProspectiveCandidates(dto, UserDto);

               if (!string.IsNullOrEmpty(ErrorString)) return BadRequest(new ApiResponse(404, ErrorString));

               return Ok();
          }
          
     }
}