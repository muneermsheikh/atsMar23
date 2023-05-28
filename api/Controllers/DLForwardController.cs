using api.Errors;
using api.Extensions;
using core.Dtos.Admin;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Entities.Tasks;
using core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     public class DLForwardController : BaseApiController
     {
          private readonly IDLForwardService _dlfwdService;
          private readonly UserManager<AppUser> _userManager;
          private readonly IEmployeeService _empService;
          private readonly ITaskService _taskService;
          public DLForwardController(IDLForwardService dlfwdService, UserManager<AppUser> userManager, ITaskService taskService, IEmployeeService empService)
          {
               _taskService = taskService;
               _empService = empService;
               _userManager = userManager;
               _dlfwdService = dlfwdService;
          }

          [Authorize]  //(Roles = "DocumentControllerAdmin, HRSupervisor, HRExecutive, HRTrainee" )]
          [HttpPost]
          public async Task<ActionResult<string>> ForwardDLToAgents(DLForwardToAgent dlforward)
          {
               if (dlforward==null) return BadRequest(new ApiResponse(400, "bad request - null object"));
               if(dlforward.ProjectManagerId==0) return BadRequest("Project Manager for the Demand Letter not defined");
               
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               
               var errorString = await _dlfwdService.ForwardDLToAgents(dlforward, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName, loggedInUser.Email);
               if(string.IsNullOrEmpty(errorString)) return Ok();
               
               return BadRequest(new ApiResponse(404, errorString));
          }

          [HttpGet("activeDL")]
          public async Task<ICollection<DLForwardToAgent>> GetDLForwardsForActiveDL()
          {
               return await _dlfwdService.DLForwardsForActiveDLs();
          }

          [HttpGet("byorderid/{orderid}")]
          public async Task<ActionResult<ICollection<DLForwardToAgent>>> GetDLForwardsForAnOrderId(int orderid)
          {
               var dto = await _dlfwdService.DLForwardsForDL(orderid);

               if (dto==null) return NotFound(new ApiResponse(402, "No DL Forward records exist for the selected order"));
               
               var dtos = new List<DLForwardToAgent>();
               dtos.Add(dto);
               return Ok(dtos);
          }

          [HttpGet("associatesforwardedForOrderId/{orderid}")]
          public async Task<ActionResult<DLForwardedDateDto>> DLCategoriesForwardedToAgents (int orderid)
          {
               var forwarded = await _dlfwdService.OrderItemForwardedToStats(orderid);

               if(forwarded==null) return NotFound(new ApiResponse(404, "Categories not forwarded to any agents"));

               return Ok(forwarded);
          }
     
          [Authorize]  //(Roles="DocumentControllerAdmin, Admin")]
          [HttpPost("addtaskdltohr/{orderid}")]
          public async Task<ActionResult<ApplicationTask>> NewDLTaskForHRDept(int orderid)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User); 
               var t = await _taskService.NewDLTaskForHRDept(orderid, loggedInUser.loggedInEmployeeId);
               if(t==null) return BadRequest(new ApiResponse(402, "Failed to create task for the DL - check it is not created earlier"));

               return Ok(t);
          }
     }
}