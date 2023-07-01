using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Entities.HR;
using core.Entities.Identity;
using core.Interfaces;
using core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using core.Params;

namespace api.Controllers
{
     [Authorize]
     public class CVRefController : BaseApiController
     {
          private readonly ICVRefService _cvrefService;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IMapper _mapper;
          private readonly ITaskService _taskService;
          private readonly IConfiguration _config;
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmployeeService _empService;
          public CVRefController(ICVRefService cvrefService, IUnitOfWork unitOfWork, IEmployeeService empService,
          IMapper mapper, ITaskService taskService, IConfiguration config, UserManager<AppUser> userManager)
          {
               _empService = empService;
               _userManager = userManager;
               _config = config;
               _taskService = taskService;
               _mapper = mapper;
               _unitOfWork = unitOfWork;
               _cvrefService = cvrefService;
          }

          [HttpGet("orderitem/{orderitemid}")]
          public async Task<ActionResult<ICollection<CVRef>>> GetReferralsOfOrderItemId(int orderitemid)
          {
               var refs = await _cvrefService.GetReferralsOfOrderItemId(orderitemid);
               if (refs == null) return NotFound(new ApiResponse(404, "No record found"));
               return Ok(refs);
          }

          [HttpGet("referralsofcandidate/{candidateid}")]
          public async Task<ActionResult<ICollection<CVRef>>> GetReferralsOfACandidate(int candidateid)
          {
               var refs = await _cvrefService.GetReferralsOfACandidate(candidateid);
               if (refs == null) return NotFound(new ApiResponse(404, "No record found"));
               return Ok(refs);
          }

          [HttpGet("cvref/{cvrefid}")]
          public async Task<ActionResult<CVRef>> GetCVRef(int cvrefid)
          {
               var cvref = await _cvrefService.GetReferralById(cvrefid);

               if (cvref == null)
               {
                    return NotFound(new ApiResponse(404, "Not Found"));
               }
               else
               {
                    return Ok(cvref);
               }
          }

          //[Authorize]    //(Roles="Admin, DocumentControllerAdmin, HRManager, HRSupervisor, HRExecutive")]
          [HttpGet("{candidateid}/{orderitemid}")]
          public async Task<ActionResult<CVRef>> GetReferralsOfCandidateAndOrderItem(int candidateid, int orderitemid)
          {
               var cvref = await _cvrefService.GetReferralByCandidateAndOrderItem(candidateid, orderitemid);
               if (cvref == null) return NotFound(new ApiResponse(404, "No record found"));
               return Ok(cvref);
          }

          [HttpGet("cvsreferredPaginated")]
          public async Task<ActionResult<Pagination<CVReferredDto>>> GetCVsReferredPaginated([FromQuery] CVRefSpecParams refParams )
          {
               var refs = await _cvrefService.GetCVReferredDto(refParams);
               //if(refs==null) return Ok(new ApiResponse(404, "No data to report"));
               return Ok(refs);
          }
          
          [HttpGet("cvrefwithdeploys/{cvrefid}")]
          public async Task<ActionResult<CVReferredDto>> GetCVRefWithDeploys(int cvrefid)
          {
               var dto = await _cvrefService.GetCVRefWithDeploys(cvrefid);

               return Ok(dto);
          }
          //[Authorize]    //(Roles="DocumentControllerAdmin, HRManager")]
          [HttpPost]
          public async Task<ActionResult<MessagesDto>> MakeReferrals(ICollection<int> CVReviewIds)
          {
               var loggedInDto = await GetLoggedInUserDto();
               var msgs = await _cvrefService.MakeReferralsAndCreateTask(CVReviewIds, loggedInDto);
               if(string.IsNullOrEmpty(msgs.ErrorString)) return Ok(msgs);

               return BadRequest(new ApiResponse(404, "Failed to make the CV referrals - error: " + msgs.ErrorString));
          }

          //[Authorize(Roles="DocumentControllerAdmin, HRManager")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditAReferral(CVRef cvref)
          {
               return await _cvrefService.EditReferral(cvref);
          }

          //[Authorize(Roles="Admin, DocumentControllerAdmin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpGet("cvsreadytoforward")]
          public async Task<ActionResult<ICollection<CustomerReferralsPendingDto>>> CustomerReferralsPending()
          {
               var loggedInDto = await GetLoggedInUserDto();
               if (loggedInDto == null) return Unauthorized(new ApiResponse(401, "this option requires logged in User"));

               var pendings = await _cvrefService.CustomerReferralsPending(0);

               if (pendings==null && pendings.Count == 0) return NotFound(new ApiResponse(402, "No CVs pending for forwarding to customers"));

               return Ok(pendings);

          }
          
          private async Task<LoggedInUserDto> GetLoggedInUserDto()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser == null) {
                    loggedInUser = await _userManager.FindByEmailAsync("munir@afreenintl.in");
               } 

               var empId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               var loggedInUserDto = new LoggedInUserDto
               {
                    LoggedIAppUsername = loggedInUser.UserName,
                    LoggedInAppUserEmail = loggedInUser.Email,
                    LoggedInAppUserId = loggedInUser.Id,
                    LoggedInEmployeeId = empId,
                    HasAdminPrivilege = User.IsInRole("Admin")
               };
               return loggedInUserDto;
          }
     }
}