using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class TaskControlledServices: ITaskControlledService
    {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IValidateTaskService _validateTaskService;
          private readonly IComposeMessagesForInternalReviewHR _composeMsgHR;
          private readonly IEmailService _emailService;
          private readonly ITaskService _taskService;
          static TaskItem staticItem=new TaskItem();
          private readonly IComposeOrderAssessment _composeOrderAssessment;
          private readonly IComposeMessagesForInternalReviewHR _composeMsgInternalReview;

          public TaskControlledServices(ATSContext context, IUnitOfWork unitOfWork, ICommonServices commonServices, ITaskService taskService,
               IValidateTaskService validateTaskService, IComposeMessagesForInternalReviewHR composeMsgHR, IEmailService emailService,
               IComposeOrderAssessment composeOrderAssessment, IComposeMessagesForInternalReviewHR composeMsgInternalReview)
          {
                  
               _taskService = taskService;
               _emailService = emailService;
               _composeMsgHR = composeMsgHR;
               _validateTaskService = validateTaskService;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
               _context = context;
               _composeOrderAssessment = composeOrderAssessment;
               _composeMsgInternalReview = composeMsgInternalReview;
          }

          public async Task<ICollection<EmailMessage>> CreateTaskForHRExecOnOrderItemIds(ICollection<OrderAssignmentDto> assignments, int loggedInEmployeeId)
          {
               var tasks = new List<ApplicationTask>();
               var task = new ApplicationTask();

               foreach(var t in assignments)
               {
                    if (string.IsNullOrEmpty(t.CustomerName)) t.CustomerName=await _commonServices.CustomerNameFromCustomerId(t.CustomerId);
                    if (string.IsNullOrEmpty(t.CategoryName)) t.CategoryName=await _commonServices.CategoryNameFromCategoryId(t.CategoryId);
                    if (string.IsNullOrEmpty(t.ProjectManagerPosition)) t.ProjectManagerPosition=await _commonServices.GetEmployeePositionFromEmployeeId(t.ProjectManagerId);
                    if(string.IsNullOrEmpty(t.CustomerName) || string.IsNullOrEmpty(t.CategoryName) || string.IsNullOrEmpty(t.ProjectManagerPosition)) continue;
                    
                    if (t.CompleteBy.Year < 2000) t.CompleteBy = new DateTime().AddDays(7);
                    var taskitems = new List<TaskItem>();
                    
                    taskitems.Add(new TaskItem((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now, "not started", "task initiated", 
                         t.OrderId, t.OrderItemId, t.OrderNo, loggedInEmployeeId, DateTime.Now.AddDays(7), t.HrExecId,t.Quantity));

                    task = new ApplicationTask((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now, t.ProjectManagerId, t.HrExecId, (int)t.OrderId, (int)t.OrderNo,
                         (int)t.OrderItemId, "Task assigned for " + t.Quantity + " CVs of " + t.CategoryName + ", Category Reference " + t.CategoryRef + " for " + t.CustomerName, 
                         t.CompleteBy, "Not Started", 0, taskitems);
                    string ErrString = await _validateTaskService.ValidateTaskObject(task);
                    if (string.IsNullOrEmpty(ErrString)) tasks.Add(task);

               }
               
               if(tasks==null || tasks.Count==0) return null;

               foreach(var tsk in tasks) {
                    _unitOfWork.Repository<ApplicationTask>().Add(tsk);
               }
               
               var recordsAffected = await _unitOfWork.Complete();

               if (recordsAffected == 0) return null;  // throw new Exception("Failed to create the task");

               var emailMsgs = (List<EmailMessage>) await _composeMsgHR .ComposeMessagesToHRExecToSourceCVs(assignments);
               if (emailMsgs != null && emailMsgs.Count ==0) return null;

               foreach(var msg in emailMsgs)
               {
                    if ((msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmail || 
                         msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmailComposeAndSendSMS ))
                    {
                         var attachments = new List<string>();
                         _emailService.SendEmail(msg, attachments);
                    }
               }
               /*   //TODO - test for recipient particulars before allowing direct send
               if (msg != null) {
                    var attachments = new List<string>();        // TODO - should this be auto-sent?
                    msg = await _emailService.SendEmail(msg, attachments);
               }
               */
               return emailMsgs;

          }
     
          public async Task<bool> CreateTaskForHRExecOnOrderItemId_s(Order modelOrder, int loggedInEmployeeId)
          {
               //var orderassignmentdto = await GetAssignmentDtoFromItemIds(OrderItemIds);
               var ids = modelOrder.OrderItems.Select(x => x.Id).ToList();
               var existingOrderItems = await _context.OrderItems.Where(x => ids.Contains(x.Id)).ToListAsync();
               var tasks = new List<ApplicationTask>();
               int added=0;
               var orderitemid = modelOrder.OrderItems.Select(x => x.Id).FirstOrDefault();
               var CustomerName = await _context.Customers.Where(x => x.Id == modelOrder.CustomerId).Select(x => x.CustomerName).FirstOrDefaultAsync();

               var cats = await _context.Categories.Where(x => modelOrder.OrderItems.Select(x => x.CategoryId).ToList().Contains(x.Id)).Select(x => new {x.Id, x.Name}).ToListAsync();

               foreach(var model in modelOrder.OrderItems)
               {
                    var existingOrderItem = existingOrderItems.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (existingOrderItem.HrExecId != model.HrExecId) {
                         existingOrderItem.HrExecId = model.HrExecId;
                         _unitOfWork.Repository<OrderItem>().Update(existingOrderItem);

                         var task = new ApplicationTask((int)EnumTaskType.AssignTaskToHRExec, DateTime.Now,
                              loggedInEmployeeId, (int)model.HrExecId, model.OrderId, model.OrderNo,
                              model.Id, "Category Ref " + modelOrder.OrderNo + "-" + model.SrNo + " " + 
                              cats.Where(x => x.Id == model.CategoryId).Select(x => x.Name).FirstOrDefault() +
                              " total " + model.Quantity + " for " + CustomerName + " assigned to you", 
                              model.CompleteBefore.Date, "Open", 0, null);
                         //task.PostTaskAction = EnumPostTaskAction.ComposeAndSendEmail;
                    
                         _unitOfWork.Repository<ApplicationTask>().Add(task);
                         added++;
                    }
               }

               if(added == 0) return false;   // throw new Exception("no valid records found to generate order assignments");
               var recordsAffected = await _unitOfWork.Complete();

               if (recordsAffected == 0) return false;  // throw new Exception("Failed to create the task");

               var dtos = await _taskService.GetAssignmentDtoFromOrderId(modelOrder.Id);

               var msgs = (List<EmailMessage>) await _composeMsgHR.ComposeMessagesToHRExecToSourceCVs(dtos);
               
               
               /*   ** TODO ** - test for recipient particulars before allowing direct send
               if (msg != null) {
                    var attachments = new List<string>();        // TODO - should this be auto-sent?
                    msg = await _emailService.SendEmail(msg, attachments);
               }
               */               

               return recordsAffected > 0 ;
          }
    
          public async Task<MessagesDto> EditApplicationTask(ApplicationTask taskModel, int LoggedInEmployeeId, string employeeName)
          {
               //thanks to @slauma of stackoverflow
               var existingObj = _context.Tasks.Where(p => p.Id == taskModel.Id)
                    .Include(p => p.TaskItems)
                    .AsNoTracking()
                    .SingleOrDefault();

               if (existingObj == null) return null;

               _context.Entry(existingObj).CurrentValues.SetValues(taskModel);   //saves only the parent, not children

               //the children - taskitems
               //Delete children that exist in database(existingObj), but not in the model 
               foreach (var existingItem in existingObj.TaskItems.ToList())
               {
                    if (!taskModel.TaskItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.TaskItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children pressent in the model are either updated or new ones to be added
               foreach (var item in taskModel.TaskItems)
               {
                    var existingItem = existingObj.TaskItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       // Update child
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(item);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else            //insert children as new record
                    {
                         if(staticItem.TaskItemDescription==item.TaskItemDescription && staticItem.TransactionDate==item.TransactionDate) break;

                         staticItem = item;
                         if (Convert.ToDateTime((item.NextFollowupOn)).Year < 2000) item.NextFollowupOn=null;
                         if (item.UserId==0) item.UserId=LoggedInEmployeeId;
                         if(string.IsNullOrEmpty(item.UserName)) item.UserName=employeeName;

                         var newItem = new TaskItem
                         {
                              ApplicationTaskId = taskModel.Id,
                              TransactionDate = item.TransactionDate,
                              TaskTypeId = item.TaskTypeId,
                              TaskStatus = item.TaskStatus,
                              TaskItemDescription = item.TaskItemDescription,
                              OrderId = item.OrderId,
                              OrderItemId = item.OrderItemId,
                              OrderNo = item.OrderNo,
                              CandidateId = item.CandidateId,
                              UserId = item.UserId,
                              UserName=item.UserName,
                              Quantity = item.Quantity,
                              NextFollowupOn = item.NextFollowupOn,
                              NextFollowupById = item.NextFollowupById
                         };

                         existingObj.TaskItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }
               _context.Entry(existingObj).State = EntityState.Modified;

               var dto = new MessagesDto();

               if (await _context.SaveChangesAsync() > 0) {
                    if(_context.Entry(taskModel).State != EntityState.Unchanged) {
                         var msgs = await ComposeMessageFromTask(existingObj, LoggedInEmployeeId);
                         dto.emailMessages = msgs;
                    }
               } else {
                    dto.emailMessages=null;
                    dto.ErrorString = "Failed to create the messages";
               }

               return dto;
          }

          public async Task<bool> DeleteApplicationTask(int taskid)
          {
               var task = await _context.Tasks.FindAsync(taskid);
               if (task==null) return false;
               _unitOfWork.Repository<ApplicationTask>().Delete(task);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<TaskItem> CreateNewTaskItem(TaskItem taskItem)
          {
               _unitOfWork.Repository<TaskItem>().Add(taskItem);
               if (await _unitOfWork.Complete() > 0) return taskItem;
               return null;
          }

          public async Task<TaskItem> EditTaskItem(TaskItem taskItem)
          {
               var existingObj = await _context.TaskItems.FindAsync(taskItem.Id);
               if (existingObj == null) return null;

               _context.Entry(existingObj).CurrentValues.SetValues(taskItem);
               _context.Entry(existingObj).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0 ? taskItem : null;
          }

          public async Task<bool> DeleteTaskItem(int taskitemid)
          {
               var taskitem = await _context.TaskItems.FindAsync(taskitemid);
               if(taskitem==null) return false;
               _unitOfWork.Repository<TaskItem>().Delete(taskitem);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<MessagesDto> CreateNewTaskAndMsgs(ApplicationTask task, int LoggedInEmployeeId)
          {
               var dto = new MessagesDto();

               string ErrString = await _validateTaskService.ValidateTaskObject(task);
               if (!string.IsNullOrEmpty(ErrString)) {
                    dto.ErrorString = ErrString;
                    return dto;
               } 

               _unitOfWork.Repository<ApplicationTask>().Add(task);

               //if tsktypeid=14 (forwrd dl to hr head) update Orders.ForwardedToHRDeptHead field
               if(task.TaskTypeId==14 && task.OrderId != 0) {
                    var order = await _context.Orders.FindAsync(task.OrderId);
                    order.ForwardedToHRDeptOn=task.TaskDate;
                    _unitOfWork.Repository<Order>().Update(order);
               }

               var recordsAffected = await _unitOfWork.Complete();

               if (recordsAffected == 0) {
                    dto.ErrorString = "failed to create the task";
                    return dto;
               }

               var emailMsgs = await ComposeMessageFromTask(task, LoggedInEmployeeId);

               dto.emailMessages=emailMsgs;
               return dto;
          }

          private async Task<ICollection<EmailMessage>> ComposeMessageFromTask(ApplicationTask task, int LoggedInEmployeeId)
          {
               var emailMsgs = new List<EmailMessage>();
               switch (task.TaskTypeId)
               {
                    case (int)EnumTaskType.OrderCategoryAssessmentQDesign:
                         emailMsgs = (List<EmailMessage>)
                              await _composeOrderAssessment.ComposeMessagesToDesignOrderAssessmentQs((int)task.OrderId, LoggedInEmployeeId);
                         break;

                    case (int)EnumTaskType.AssignTaskToHRExec:
                         var assignmentdtos = await (from i in _context.OrderItems where i.OrderId == task.OrderId
                              join o in _context.Orders on i.OrderId equals o.Id
                              join e in _context.Employees on o.ProjectManagerId equals e.Id
                              join c in _context.Categories on i.CategoryId equals c.Id
                              select new OrderAssignmentDto(i.OrderId, o.OrderNo, o.OrderDate, o.CityOfWorking, o.ProjectManagerId,
                                   e.Position, o.Id, (int)i.HrExecId, o.OrderNo.ToString() + i.SrNo.ToString(), i.CategoryId, 
                                   c.Name, o.CustomerId, o.Customer.CustomerName,
                                   i.Quantity, i.CompleteBefore, (int)EnumPostTaskAction.OnlyComposeEmailMessage)
                              ).ToListAsync();

                         var msgsToReturn = 
                              await _composeMsgInternalReview.ComposeMessagesToHRExecToSourceCVs(assignmentdtos);
                         break;
                    case (int)EnumTaskType.CVForwardToCustomers:
                         //check unique index violations  - TaskType + candidateId + orderItemId + assignedToId
                         
                         //update CandidateAssessment.TaskIdDocControllerAdmin
                         var candidateassessment = await _context.CandidateAssessments
                              .Where(x => x.CandidateId == task.CandidateId && x.OrderItemId == task.OrderItemId)
                              .SingleOrDefaultAsync();
                         candidateassessment.TaskIdDocControllerAdmin = task.Id;
                         //** TODO ** THIS is supposed ony to return MessageDto, not to send
                         _unitOfWork.Repository<CandidateAssessment>().Update(candidateassessment);
                         await _unitOfWork.Complete();
                         break;
                    
                    default:
                         break;
               }

               return emailMsgs;
          }
                    //getTasksPaginated

          private bool IsPPNumber(string str)
          {
               var c = str.Substring(0,1);

          //if(!IsNumeric(c.Substring(1,str.Length-1))) return false;
               
               //int n = Convert.ToInt16(c);
               int n = str[0];
               if((n >= 65 && n <= 90) || (n >= 97 && n <= 122)) {
                    var restno = str.Substring(1,str.Length-1);
                    if(restno.Length >5 && IsNumeric(restno)) {
                         return true;
                    } else {
                         return false;
                    }
               } else {
                    return false;
               }
          }

          private bool IsNumeric(string c)
          {
               long number1 = 0;
               return long.TryParse(c, out number1);
          }
          public async Task<ICollection<TaskType>> GetTaskTypes()
          {
               return await _context.TaskTypes.OrderBy(x => x.Name).ToListAsync();
          }

         private async Task<ApplicationTask> GetApplicationTaskFromMobileNoFromProspective(string mobileno) {
               var qry = await (from t in _context.Tasks.Include("TaskItems")
                    join p in _context.ProspectiveCandidates on t.ResumeId equals p.ResumeId
                    where p.PhoneNo.Substring(p.PhoneNo.Length-10, 10)==mobileno
                    select t)
                    .FirstOrDefaultAsync();
               
               return qry;
          }
          public static bool IsEmailAddressValid(string email)
          {                       
               var trimmedEmail = email.Trim();
          
               if (trimmedEmail.EndsWith(".")) {
                    return false; // suggested by @TK-421
               }
               try {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == trimmedEmail;
               }
               catch {
                    return false;
               }
          
          }

     }
}