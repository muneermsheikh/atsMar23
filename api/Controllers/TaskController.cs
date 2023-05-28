using api.Errors;
using api.Extensions;
using core.Entities.Identity;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace api.Controllers
{
     [Authorize]
     public class TaskController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ITaskService _taskService;
          private readonly IEmailService _emailService;
          private readonly ITaskControlledService _taskControlledService;
          private readonly UserManager<AppUser> _userManager;
          //private readonly RoleManager<AppRole> _roleManager;

          private static ApplicationTask staticTask=new ApplicationTask();
          private static TaskItem staticTaskItem=new TaskItem();

          public TaskController(IUnitOfWork unitOfWork, 
               //RoleManager<AppRole> roleManager,
               UserManager<AppUser> userManager,
               ITaskService taskService, 
               ITaskControlledService taskControlledService,
               IEmailService emailService)
          {
               _emailService = emailService;
               _taskService = taskService;
               _taskControlledService=taskControlledService;
               _unitOfWork = unitOfWork;
               _userManager = userManager;
               //_roleManager = roleManager;
          }

          [Authorize]
          [HttpPost("getorcreate")]
          public async Task<ActionResult<ApplicationTask>> GetOrCreateApplicationTask([FromQuery]ApplicationTask task) {
               var t = await _taskService.GetOrCreateTask(task);
               if (t!=null) return Ok(t);
               return BadRequest(new ApiResponse(404, "Failed to get or create new task"));
          }

          [Authorize]
          [HttpPost("getorcreatetaskfromparams")]
          public async Task<ActionResult<ApplicationTask>> GetOrCreateApplicationTaskFrmParams(TaskParams tparams)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               var task = await _taskService.GetOrCreateTaskFromParams(tparams, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);

               if (task == null) return BadRequest(new ApiResponse(404, "Failed to get or create task"));

               return Ok(task);

          }
          [Authorize]
          [HttpPost("create")]
          public async Task<ActionResult<ApplicationTask>> CreateApplicationTask(ApplicationTask t) {
               
               //during develoment, there is a repeat call for this with same paraeters,
               //which results in database index errors, so flg will avoid this.
               /* if(staticTask.TaskDescription==t.TaskDescription && staticTask.TaskDate==t.TaskDate 
                    && staticTask.AssignedToId==t.AssignedToId && staticTask.TaskTypeId==t.TaskTypeId
                    && staticTask.OrderId == t.OrderId) return BadRequest(new ApiResponse(402, "Task already created"));

               staticTask = t;
               */
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (Convert.ToDateTime((t.CompleteBy)).Year < 2000) t.CompleteBy=null;
               if (Convert.ToDateTime(t.CompletedOn).Year < 2000) t.CompletedOn=null;
               if (string.IsNullOrEmpty(t.TaskOwnerName)) t.TaskOwnerName=loggedInUser.DisplayName;
               if (t.ResumeId=="") t.ResumeId=null;

               var task = await _taskService.GetOrCreateTask(t);
               var calledTask=t;
               if (task!=null) return Ok(task);
               return BadRequest(new ApiResponse(404, "Failed to create new task"));
          }
          
          [Authorize]
          [HttpPost]
          public async Task<ActionResult<ICollection<EmailAndSmsMessagesDto>>> CreateNewApplicationTask(ApplicationTask task)
          {
               var loggedInUser = User.GetUserIdentityUserEmployeeId();
               //verify object data
               if (task.TaskDate.Year < 2000) return BadRequest(new ApiResponse(404, "Task Date not set"));
               if (Convert.ToDateTime(task.CompleteBy).Year < 2000) return BadRequest(new ApiResponse(404, "Task Completion Date not set"));
               if (task.TaskOwnerId == 0) return BadRequest(new ApiResponse(404, "Bad Request - Task Owner not defined"));
               if (string.IsNullOrEmpty(task.TaskStatus)) return BadRequest(new ApiResponse(404, "Bad Request - task status not provided"));
               if (string.IsNullOrEmpty(task.TaskDescription)) return BadRequest(new ApiResponse(404, "Task Description cannot be blank"));
               if (task.AssignedToId == 0) return BadRequest(new ApiResponse(404, "Task not assigned to any one"));

               // ** TODO ** - verify assignedToId and TaskOwnerId exist
               
               var emailMessages = await _taskControlledService.CreateNewTaskAndMsgs(task, loggedInUser);
               var AttachmentFilePaths = new List<string>();
               if (emailMessages != null &&
                   task.PostTaskAction != EnumPostTaskAction.OnlyComposeEmailAndSMSMessages
                   && task.PostTaskAction != EnumPostTaskAction.OnlyComposeEmailMessage)   //Send involed
               {
                    foreach (var msg in emailMessages.emailMessages)
                    {
                        _emailService.SendEmail(msg, AttachmentFilePaths);
                    }
               }

               return Ok(emailMessages);

          }

          [HttpPut]
          public async Task<ActionResult<MessagesDto>> EditApplicationTask(ApplicationTask task)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               return await _taskControlledService.EditApplicationTask(task, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
          }

          [HttpDelete("task/{taskid}")]
          public async Task<bool> DeleteApplicationTask(int taskid)
          {
                if(await _taskControlledService.DeleteApplicationTask(taskid)) {
                    return true;
               } else {
                    return false;
               }
          }

          [HttpGet("tasktypes")]
          public async Task<ActionResult<ICollection<TaskType>>> GetTaskTypes()
          {
               var t = await _taskControlledService.GetTaskTypes();    
               if (t==null) return Ok(null);
               return Ok(t);
          }

     
          [Authorize]
          [HttpGet("paginatedtasksOfloggedinuser")]
          public async Task<ActionResult<Pagination<ApplicationTaskDto>>> GetPendingTasksOfLoggedInUserId ([FromQuery]TaskParams tParams)
          {
               var user = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (user==null) return BadRequest(new ApiResponse(402, "No User Claim found"));
               var isAdminRole = User.UserHasTheRole("Admin");
               int loggedInUser = isAdminRole ? 0: user.loggedInEmployeeId;      //if admin role, return tasks of all users
               var emps = await _taskService.GetTasksPaginated(tParams, user.loggedInEmployeeId);
               
               if (emps == null || emps.Count == 0) return null;  //BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(emps);
          }
     
          [HttpGet("wopagination")]
          public async Task<ActionResult<ICollection<ApplicationTaskDto>>> GetApplicationTasksWOPagination([FromQuery]TaskParams taskParams)
          {
               var tasks = await _taskService.GetTasksByParams(taskParams);
               if (tasks == null || tasks.Count == 0) return BadRequest(new ApiResponse(404, "Failed to retrieve any tasks"));

               return Ok(tasks);
          }

          [Authorize]
          [HttpPut("applicationtask/{applicationTaskId}")]
          public async Task<ActionResult<bool>> SetApplicationTaskAsCompleted(int applicationTaskId)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               var dateCompleted = DateTime.Now;

               return await _taskService.SetApplicationTaskStatus(applicationTaskId, dateCompleted, "Completed", 
                    loggedInUser.DisplayName, loggedInUser.loggedInEmployeeId);
          }

          [Authorize]
          [HttpPut("applicationtask/{applicationTaskId}/{dateOfCancellation}")]
          public async Task<ActionResult<bool>> SetApplicationTaskAsCanceled(int applicationTaskId, DateTime dateOfCancellation)
          {
               var userName = User.GetUsername();
               var userId = User.GetIdentityUserId();
               return await _taskService.SetApplicationTaskStatus(applicationTaskId, dateOfCancellation, "Canceled", userName, userId);
          }

          [HttpPost("item")]
          public async Task<ActionResult<TaskItem>> AddATaskItem(TaskItem taskItem)
          {
               if(staticTaskItem.TaskItemDescription == taskItem.TaskItemDescription && staticTaskItem.TransactionDate == taskItem.TransactionDate ) return Ok();

               staticTaskItem = taskItem;
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (Convert.ToDateTime(taskItem.NextFollowupOn).Year < 2000) taskItem.NextFollowupOn=null;
               taskItem.UserName=loggedInUser.DisplayName;
               taskItem.UserId=loggedInUser.loggedInEmployeeId;
               if(taskItem.TransactionDate.Year < 2000) taskItem.TransactionDate=DateTime.Now;

               var item = await _taskControlledService.CreateNewTaskItem(taskItem);
               if (item != null) return Ok(item);
               return BadRequest(new ApiResponse(404, "Failed to add the task item"));
          }

          [HttpPut("item")]
          public async Task<ActionResult<TaskItem>> EditTaskItem(TaskItem taskItem)
          {
               var t = await _taskControlledService.EditTaskItem(taskItem);

               if (t == null) return BadRequest(new ApiResponse(404, "Failed to edit the task item"));

               return Ok(t);
          }

          [HttpDelete("taskitem/{taskitemid}")]
          public async Task<bool> DeleteTaskItem(int taskitemid)
          {
               if(await _taskControlledService.DeleteTaskItem(taskitemid)) {
                    return true;
               } else {
                    return false;
               }
          }

          [HttpGet("byid/{taskid}")]
          public async Task<ActionResult<ApplicationTask>> GetTaskById(int taskid) {
               
               var taskParams = new TaskParams();
               taskParams.TaskId = taskid;
               var task = await _taskService.GetTaskByParams(taskParams);
               if(task == null) return NotFound(new ApiResponse(404, "Record Not Found"));
               return Ok(task);
          }

          [HttpGet("getorcreatebyresumeid/{resumeid}")]
          public async Task<ActionResult<ApplicationTask>> GetTaskByResumeId(string resumeid) {
               
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               var tParams = new TaskParams();
               tParams.ResumeId=resumeid;
               var task = await _taskService.GetOrCreateTaskFromParams(tParams, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
               
               if(task == null) return NotFound(new ApiResponse(404, "Record Not Found"));
               
               return Ok(task);
          }


     }
}