using api.Errors;
using api.Extensions;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Interfaces;
using core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
    public class OrderAssignmentController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        //private readonly ICommonServices _commonServices;
        private readonly string _loggedInUserEmail;
        private readonly IOrderAssignmentService _orderAssignmentService;
        private readonly ITaskService _taskService;
        private readonly ITaskControlledService _taskControlledService;
        public OrderAssignmentController(
            IOrderAssignmentService orderAssignmentService,
            UserManager<AppUser> userManager,
            ITaskService taskService,
            ITaskControlledService taskControlledService)
        {
            _orderAssignmentService = orderAssignmentService;
            _userManager = userManager;
            _loggedInUserEmail = User.GetIdentityUserEmailId();
            _taskService = taskService;
            _taskControlledService=taskControlledService;
        }


        //assign task to HR Sup or HR Manager to design AssessmentQ for the 
        //order, if the flag RequireAssess is set to true
        [HttpPost("design/{orderId}")]
        public async Task<ActionResult<EmailAndSmsMessagesDto>> AssignTaskToDesignOrderAssessmentQ(int orderId)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            //var loggedInAppUserEmail = User.GetIdentityUserEmailId();

            var msg = await _orderAssignmentService.DesignOrderAssessmentQs(orderId, loggedInUser.loggedInEmployeeId);
            
            if (msg != null) return Ok(msg);

            return BadRequest(new ApiResponse (404, "Failed to create tasks for the HR Executives"));
            
        }

        [HttpDelete("hrexec/{taskid}")]
        public async Task<ActionResult<bool>> DeleteHRExecAssignment(int taskid)
        {
            return await _orderAssignmentService.DeleteHRExecAssignment(taskid);
        }

        [HttpPost("orderitems")]
        public async Task<ActionResult<ICollection<EmailMessage>>> AssignHRExecTasks(ICollection<OrderAssignmentDto> orderassignments)
        {
            if (orderassignments==null || orderassignments.Count() ==0) return BadRequest(new ApiResponse(400, "no data provided"));
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            
            var msgs = await _taskControlledService.CreateTaskForHRExecOnOrderItemIds(orderassignments, loggedInUser.loggedInEmployeeId);
            if (msgs!=null && msgs.Count > 0) return Ok(msgs);
            return BadRequest(new ApiResponse(404, "failed to create tasks"));
        }

    }
}