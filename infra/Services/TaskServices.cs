using AutoMapper;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class TaskServices : ITaskService
     {
          private readonly ICommonServices _commonServices;
          private readonly ATSContext _context;
          private readonly int EmployeeIdDocController;
          private readonly int TargetDays_HRSupToReviewCV;
          private readonly int TargetDays_DocControllerToFwdCV;
          public IConfiguration Config { get; set; }
          private readonly IComposeMessagesForHR _composeMsgHR;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IEmailService _emailService;
          private readonly IEmployeeService _empService;
          private readonly IMapper _mapper;
          private readonly IValidateTaskService _validateTaskService;
          
          
          public TaskServices(ICommonServices commonServices, 
               ATSContext context, 
               IConfiguration config, 
               IMapper mapper, 
               IComposeMessagesForHR composeMsgHR, 
               IUnitOfWork unitOfWork, 
               IEmailService emailService, 
               IEmployeeService empService, IValidateTaskService validateTaskService)
          {
               
               
               _empService = empService;
               _mapper = mapper;
               _emailService = emailService;
               _unitOfWork = unitOfWork;
               _composeMsgHR = composeMsgHR;
               _context = context;
               _commonServices = commonServices;
               _validateTaskService = validateTaskService;

               EmployeeIdDocController = Convert.ToInt32(config.GetSection("EmpDocControllerAdminId").Value);
               TargetDays_HRSupToReviewCV = Convert.ToInt32(config.GetSection("TargetDays_HRSupToReviewCV").Value);
               TargetDays_DocControllerToFwdCV = Convert.ToInt32(config.GetSection("TargetDays_DocControllerToFwdCV").Value);

          }

          public async Task<ICollection<OrderAssignmentDto>> GetAssignmentDtoFromOrderId(int OrderId)
          {
               var assignmentdtos = await (from i in _context.OrderItems where i.OrderId == OrderId
                    join o in _context.Orders on i.OrderId equals o.Id
                    join e in _context.Employees on o.ProjectManagerId equals e.Id
                    join c in _context.Categories on i.CategoryId equals c.Id
                    select new OrderAssignmentDto(i.OrderId, o.OrderNo, o.OrderDate, o.CityOfWorking, o.ProjectManagerId,
                         e.Position, o.Id, (int)i.HrExecId, o.OrderNo.ToString() + i.SrNo.ToString(), i.CategoryId, 
                         c.Name, o.CustomerId, o.Customer.CustomerName,
                         i.Quantity, i.CompleteBefore, (int)EnumPostTaskAction.OnlyComposeEmailMessage)
                    ).ToListAsync();     
               return assignmentdtos;
          }
  
          public async Task<ApplicationTask> CreateTask(ApplicationTask task)
          {
               var t = new ApplicationTask();

               if(task.Id != 0) return null;
               if(task.AssignedToId==0) return null;
               if(string.IsNullOrEmpty(t.AssignedToName)) task.AssignedToName = await _empService.GetEmployeeNameFromEmployeeId(t.AssignedToId);
               _unitOfWork.Repository<ApplicationTask>().Add(task);

               if (await _unitOfWork.Complete() > 0) return task;

               return null;
          }     

          public async Task<ApplicationTask> GetOrCreateTask(ApplicationTask task)
          {
               var t = new ApplicationTask();

               if(task.Id != 0) {
                    t = await _context.Tasks.Where(x => x.Id==task.Id).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } else if (task.HistoryItemId !=0 && task.HistoryItemId != null) {
                    t = await _context.Tasks.Where(x => x.HistoryItemId == task.HistoryItemId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } else if (!string.IsNullOrEmpty(task.ResumeId)) {
                    t = await _context.Tasks.Where(x => x.ResumeId == task.ResumeId).FirstOrDefaultAsync();
               }

               if(t.Id > 0) return t;

               if(task.AssignedToId==0) return null;
               if(string.IsNullOrEmpty(t.AssignedToName)) task.AssignedToName = await _empService.GetEmployeeNameFromEmployeeId(t.AssignedToId);
               _unitOfWork.Repository<ApplicationTask>().Add(task);

               if (await _unitOfWork.Complete() > 0) return task;

               return null;
          }     
          
          public async Task<ApplicationTask> GetTaskByParams(TaskParams tparams) {
               var task = new ApplicationTask();

               if(tparams.TaskId!=0) {
                    task=await _context.Tasks.Where(x => x.Id == tparams.TaskId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } else if(tparams.OrderItemId != 0 && tparams.CandidateId !=0 && tparams.TaskTypeId != 0) {
                    task=await _context.Tasks.Where(x => x.OrderItemId == tparams.OrderItemId && 
                         x.AssignedToId ==  tparams.CandidateId && x.TaskTypeId == 
                         (int)EnumTaskType.AssignTaskToHRExec).FirstOrDefaultAsync();
               } else if (!string.IsNullOrEmpty(tparams.ResumeId)) {
                    task = await _context.Tasks.Where(x => x.ResumeId == tparams.ResumeId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } 
               if(task.Id==0) return null;

               return task;
               
          }
          
          public async Task<ApplicationTask> GetOrCreateTaskFromParams(TaskParams tparams, int loggedinUserId, string LoggedInUserName)
          {
               var task = new ApplicationTask();
               var cand = new Candidate();
               var prospective = new ProspectiveCandidate();
               if(tparams.TaskId > 0) {
                    task = await _context.Tasks.Where(x => x.Id == tparams.TaskId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } else if(tparams.ApplicationNo > 0 ) {
                    task = await _context.Tasks.Where(x => x.ApplicationNo==tparams.ApplicationNo).Include(x => x.TaskItems).FirstOrDefaultAsync();
               } else if (tparams.CandidateId > 0) {
                    task = await _context.Tasks.Where(x => x.CandidateId==tparams.CandidateId).Include(x => x.TaskItems).FirstOrDefaultAsync();

               } else if (!string.IsNullOrEmpty(tparams.ResumeId) ) {
                    task = await _context.Tasks.Where(x => x.ResumeId==tparams.ResumeId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               }
               
               if(task != null && task.Id==0) {
                    if(tparams.ApplicationNo > 0) {
                         cand=await _context.Candidates.Where(X => X.ApplicationNo==tparams.ApplicationNo).FirstOrDefaultAsync();
                    } else if (tparams.CandidateId > 0) {
                         cand=await _context.Candidates.FindAsync(tparams.CandidateId);
                    } else if (!string.IsNullOrEmpty(tparams.ResumeId)) {
                         prospective=await _context.ProspectiveCandidates.Where(X => X.ResumeId==tparams.ResumeId).FirstOrDefaultAsync();
                         if (prospective != null) {
                         task = new ApplicationTask {TaskTypeId = (int) EnumTaskType.ProspectiveCandidateFollowup, TaskDate =  DateTime.Today, 
                              TaskOwnerId = loggedinUserId, AssignedToId = 0, CompleteBy = DateTime.Today.AddDays(2),
                              PersonType = "Prospective", TaskOwnerName = LoggedInUserName, ResumeId = tparams.ResumeId,
                              TaskDescription=prospective.CandidateName + " - prospective candidate, resumeId=" + tparams.ResumeId};
                         }
                    }
               
                    if (cand.Id > 0) {
                         task = new ApplicationTask{CandidateId=cand.Id, AssignedToName = LoggedInUserName, 
                              TaskTypeId = tparams.TaskTypeId, TaskDate = DateTime.Now, TaskOwnerId  = loggedinUserId, 
                              AssignedToId = tparams.AssignedToId, CompleteBy = DateTime.Today.AddDays(2),
                              ApplicationNo = tparams.ApplicationNo, TaskDescription="Edit this description"};
                    }
                    
                    _unitOfWork.Repository<ApplicationTask>().Add(task);
                    if (await _unitOfWork.Complete() > 0) return task;
               }
               
               if(task == null) return null;

               if(task.Id ==0 ) return null;

               return task;
          }
          
          //creates Task, updates DL.ForwardedToHRDeptHead, composes email msg to HRDeptHead and saves all to DB
          public async Task<ApplicationTask> NewDLTaskForHRDept(int orderid, int loggedInEmployeeId)
          {
               //CHECK FOR THE CONSTRAINTS ORDERID + TASKTYPEID=14 UNIQUE CONSTRAINT
               var taskdate = await _context.Tasks.Where(x => x.TaskTypeId==14 && x.OrderId==orderid).FirstOrDefaultAsync();
               if (taskdate!=null) return null;

               var oBriefItems = await (from o in _context.Orders where o.Id == orderid 
                    join i in _context.OrderItems on o.Id equals i.OrderId
                    join aq in _context.OrderItemAssessments on i.Id equals aq.OrderItemId into assessQDesigned 
                    from aqDesigned in assessQDesigned.DefaultIfEmpty()
                    join cat in _context.Categories on i.CategoryId equals cat.Id 
                    join c in _context.Customers on o.CustomerId equals c.Id into cust 
                    from customer in cust.DefaultIfEmpty()
                    select new OrderItemBriefDto{
                         Id = o.Id, OrderItemId = i.Id, RequireInternalReview = i.RequireInternalReview,
                         OrderId = o.Id, OrderNo = o.OrderNo, OrderDate=o.OrderDate,
                         CustomerName = customer.CustomerName, AboutEmployer= customer.Introduction,
                         CategoryId = i.CategoryId, CategoryName=cat.Name, 
                         CategoryRef=o.OrderNo + "-" + i.SrNo + "-" + cat.Name,
                         Quantity = i.Quantity, Status = o.Status, JobDescription = i.JobDescription, 
                         Remuneration =  i.Remuneration,  AssessmentQDesigned = aqDesigned != null})
                    .ToListAsync();
               var ordernondt = oBriefItems.Select(x => new{x.OrderNo, x.OrderDate, x.CustomerName}).FirstOrDefault();
               
               //var HRDeptHeadId = 10;  // Environment.GetEnvironmentVariable("HRDeptHeadId");
               var intHRDeptHeadId=10; //string.IsNullOrEmpty(HRDeptHeadId) ? 0: Convert.ToInt32(HRDeptHeadId);
               var intTaskTypeId = (int)EnumTaskType.AssignDLToHRDeptHead;

               var dtNow = DateTime.Now;

               var t = new ApplicationTask(intTaskTypeId, dtNow,
                    intHRDeptHeadId, loggedInEmployeeId, orderid, ordernondt.OrderNo, "Order No.:" + ordernondt.OrderNo +
                         " dated " + ordernondt.OrderDate + " from " + ordernondt.CustomerName +  
                    " is assigned to you for execution within the period allotted." +
                    "  Pl assign necessar tasks to relevant HR Executives through the system.", 
                    dtNow.AddDays(7), EnumPostTaskAction.OnlyComposeEmailAndSMSMessages);
               //_context.Entry(t).State=EntityState.Added;
               _context.Tasks.Add(t);

               var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(loggedInEmployeeId);
               var recipientObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(intHRDeptHeadId);

               var msgs = _composeMsgHR.ComposeEmailMsgForDLForwardToHRHead(oBriefItems, senderObj, recipientObj);
               if(msgs!=null) {
                    //_context.Entry(msgs).State = EntityState.Added;
                    _context.EmailMessages.Add(msgs);
               }

               //update Orders.ForwaredToRDeptOn table
               var ord = await _context.Orders.Where(x => x.Id==orderid).FirstOrDefaultAsync();
               ord.ForwardedToHRDeptOn=dtNow;
               _context.Entry(ord).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0) return t;

               return null;
               
          }

          


 //task messaages         
public async Task<Pagination<ApplicationTaskDto>> GetTasksPaginated(TaskParams taskParams, int loggedInUserId)
          {
               var qry = (from t in _context.Tasks where t.AssignedToId==loggedInUserId || t.TaskOwnerId== loggedInUserId && t.TaskStatus != "completed"
                    join o in _context.Employees on t.TaskOwnerId equals o.Id
                    join a in _context.Employees on t.AssignedToId equals a.Id
                    orderby t.TaskDate
                    select new ApplicationTaskDto {
                         Id=t.Id, TaskTypeId=(int)t.TaskTypeId, TaskDate = t.TaskDate, TaskStatus=t.TaskStatus,
                         TaskOwnerId=t.TaskOwnerId, TaskOwnerName=o.KnownAs, AssignedToId=t.AssignedToId, AssignedToName=a.KnownAs,
                         TaskDescription = t.TaskDescription, CompleteBy=(DateTime)t.CompleteBy
                    }).AsQueryable(); 
               
               if (taskParams.OrderId > 0) qry = qry.Where(x => x.OrderId == taskParams.OrderId);
               if(taskParams.OrderItemId.HasValue) qry = qry.Where(x => x.OrderItemId == taskParams.OrderItemId);
               if(taskParams.CandidateId.HasValue && taskParams.PersonType?.ToLower() == "candidate") 
                    if(taskParams.PersonType == "candidate") qry = qry.Where(x => x.CandidateId == taskParams.CandidateId);
               if(taskParams.TaskOwnerId > 0) qry = qry.Where(x => x.TaskOwnerId == taskParams.TaskOwnerId);
               if(taskParams.AssignedToId > 0) qry = qry.Where(x => x.AssignedToId == taskParams.AssignedToId);
               

               var totalCount = await qry.CountAsync();

               var data = await qry.Skip(taskParams.PageSize * (taskParams.PageIndex-1)).Take(taskParams.PageSize).ToListAsync();

               return new Pagination<ApplicationTaskDto>(taskParams.PageIndex, taskParams.PageSize,totalCount,data);
          }

//getTaskCollection
          public async Task<ICollection<ApplicationTask>> GetTasksByParams(TaskParams taskParams)
          {
               var qry = (from t in _context.Tasks
                    join o in _context.Employees on t.TaskOwnerId equals o.Id
                    join a in _context.Employees on t.AssignedToId equals a.Id
                    orderby t.TaskDate
                    select new ApplicationTask {
                         Id=t.Id, TaskTypeId=(int)t.TaskTypeId, TaskDate = t.TaskDate, TaskStatus=t.TaskStatus,
                         TaskOwnerId=t.TaskOwnerId, TaskOwnerName=o.KnownAs, AssignedToId=t.AssignedToId, AssignedToName=a.KnownAs,
                         TaskDescription = t.TaskDescription, CompleteBy=(DateTime)t.CompleteBy
                    }).AsQueryable(); 
               
               if (taskParams.OrderId > 0) qry = qry.Where(x => x.OrderId == taskParams.OrderId);
               if(taskParams.OrderItemId.HasValue) qry = qry.Where(x => x.OrderItemId == taskParams.OrderItemId);
               if(taskParams.CandidateId.HasValue && taskParams.PersonType?.ToLower() == "candidate") 
                    if(taskParams.PersonType == "candidate") qry = qry.Where(x => x.CandidateId == taskParams.CandidateId);
               if(taskParams.TaskOwnerId > 0) qry = qry.Where(x => x.TaskOwnerId == taskParams.TaskOwnerId);
               if(taskParams.AssignedToId > 0) qry = qry.Where(x => x.AssignedToId == taskParams.AssignedToId);
               if(taskParams.TaskTypeId.HasValue) qry = qry.Where(x => x.TaskTypeId == (int)taskParams.TaskTypeId);

               var data = await qry.ToListAsync();

               return data;

          }

		public async Task<bool> SetApplicationTaskStatus(int ApplicationTaskId, DateTime dateOfStatus, string TaskStatus, string UserName, int AppUserId)
		{
			var task = await _context.Tasks.Where(x => x.Id==ApplicationTaskId).Include(x => x.TaskItems).FirstOrDefaultAsync();
               if(task==null) return false;

               task.TaskStatus=TaskStatus;
               _context.Entry(task).State=EntityState.Modified;

               var newTaskItem = new TaskItem((int)task.TaskTypeId, dateOfStatus, TaskStatus, 
                    "Task Updated to :" + TaskStatus + " by " + UserName, (int)task.OrderId,
                    (int)task.OrderItemId, (int)task.OrderNo, AppUserId, null, 0,0);
               _context.Entry(newTaskItem).State=EntityState.Added;

               return await _context.SaveChangesAsync() > 0;
               
		}
	}
}