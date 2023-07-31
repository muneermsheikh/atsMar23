using core.Entities;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using core.Params;
using core.Dtos.Admin;

namespace infra.Services
{
     public class CVRefService : ICVRefService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly ICommonServices _commonService;
          private readonly IConfiguration _config;
          private readonly IEmployeeService _empService;
          private readonly int _docControllerAdminId;
          private readonly int _TargetDays_FollowUpWithClientForSelection;
          private readonly bool _ByepassCVReviewLevels;
          private readonly IComposeMessagesForAdmin _composeMsgAdmin;
          
          private readonly IEmailService _emailService;
          private readonly ITaskService _taskService;
          public CVRefService(IUnitOfWork unitOfWork, ATSContext context, 
               ICommonServices commonService, IEmailService emailService,
               IConfiguration config, IComposeMessagesForAdmin composeMsgAdmin, 
               IEmployeeService empService, ITaskService taskService)
          {
               _composeMsgAdmin = composeMsgAdmin;
               _emailService = emailService;
               _empService = empService;
               _config = config;
               _commonService = commonService;
               _context = context;
               _unitOfWork = unitOfWork;
               _taskService = taskService;
               _docControllerAdminId = Convert.ToInt32(config.GetSection("EmpDocControllerAdminId").Value);
               _ByepassCVReviewLevels = config.GetSection("ByepassCVReviewLevels").Value.ToLower() == "true" ? true : false;
               _TargetDays_FollowUpWithClientForSelection = Convert.ToInt32(config.GetSection("TargetDays_FollowUpWithClientForSelection").Value);
          }

          public async Task<bool> EditReferral(CVRef cVRef)
          {
               var refStatus = await _context.CVRefs
               .Where(x => x.Id == cVRef.Id).Select(x => x.RefStatus)
               .FirstOrDefaultAsync();

               if (refStatus != (int)EnumCVRefStatus.Referred) return false;    //ref status changed, so no edits

               _unitOfWork.Repository<CVRef>().Update(cVRef);

               return (await _unitOfWork.Complete() > 0);
          }

          public async Task<CVRef> GetReferralById(int cvrefid)
          {
               return await _context.CVRefs.FindAsync(cvrefid);
          }
          public async Task<CVRef> GetReferralByCandidateAndOrderItem(int candidateId, int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
               .FirstOrDefaultAsync();
          }

          public async Task<ICollection<CVRef>> GetReferralsOfACandidate(int candidateId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId)
               .OrderBy(x => x.ReferredOn)
               .ToListAsync();
          }

          public async Task<ICollection<CVRef>> GetReferralsOfOrderItemId(int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.OrderItemId == orderItemId)
               .OrderBy(x => x.ReferredOn).ToListAsync();
          }

          //post actions - after inserting CVRef record
          //Update owner task of the DocController Task to forward the CV
          public async Task<MessagesDto> MakeReferralsAndCreateTask(ICollection<int> CandidateAssessmentIds, LoggedInUserDto loggedInUserDto)
          {
               //1. create CVRef records, 
               //2. Mark DocAdmin Task Id as completed because of 1
               //3 - crreate Task in the name of DocControllerAdmin to PHYSICALLY forward CV
               //4. creates task for DocControllerAdmin to follwwup for selection, 
               //        CREATING TASK also composes Messages, if he task.createMessage Flag is on
               //5. updates CandidaateAssessment.TaskIdDocControllerAdmin  and candidateAssessment.CVRefId 
               //6 - compose CV Forwarding messages
               
               //todo - implement CVRefRestriction checking
               var dto = new MessagesDto();
               var AssessmentIds = new List<int>();     //required by the client to delete the pending AssessmentIs for CV forward
               DateTime dateTimeNow = DateTime.Now;
               
               var cvrefs = new List<CVRef>();         //collection to compose messages in stage 6
               int DocControllerAdminId=4;

               //following will get assessment records that do not TaskDocumentControllerAdminId set - it is set only when cvs are sent to the client, task created and the task id registered as TaskIdDocontrollerAdmin
               var shortlistedCVsNotReferred = await _context.CandidateAssessments.Where(x => CandidateAssessmentIds.Contains(x.Id) && x.CvRefId == 0 ).ToListAsync();
               var dbChanged=false;          //if true, it writes to DB at line 167

               if (shortlistedCVsNotReferred == null || shortlistedCVsNotReferred.Count == 0) return null;

               //extract data for writing to CVRef table and to compose messages
               var itemdetails = await (from r in _context.CandidateAssessments where shortlistedCVsNotReferred.Select(x => x.Id).ToList().Contains(r.Id) 
                    join i in _context.OrderItems on r.OrderItemId equals i.Id 
                    //join cat in _context.Categories on i.CategoryId equals cat.Id  
                    join ordr in _context.Orders on i.OrderId equals ordr.Id 
                    //join c in _context.Customers on ordr.CustomerId equals c.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    join lst in _context.ChecklistHRs on new {a=cand.Id, b=i.Id} equals new {a=lst.CandidateId, b=lst.OrderItemId} into chklst
                    from checklist in chklst.DefaultIfEmpty()
                    select new {
                         AsssessmentId=r.Id,
                         OrderItemId=i.Id,
                         CategoryId = i.CategoryId,
                         OrderId = ordr.Id,
                         OrderNo = ordr.OrderNo,
                         CustomerName = ordr.Customer.CustomerName,
                         CategoryName = i.Category.Name,
                         CandidateId = cand.Id,
                         SrNo = i.SrNo, 
                         Ecnr = cand.Ecnr,
                         ApplicationNo = cand.ApplicationNo,
                         CandidateName = cand.FullName,
                         HRExecId = i.HrExecId?? 0,
                         CandidateAssessmentId = r.Id,
                         candAssessment = r,
                         DocControllerAdminTaskId = r.TaskIdDocControllerAdmin,
                         ChargesAgreed = checklist==null ? 0 : checklist.ChargesAgreed,
                         Charges = checklist.Charges   /* checklist == null || checklist.Charges == 0 
                              ? "Undefined" 
                              : checklist == null ? "undefined" :  checklist.Charges == checklist.ChargesAgreed
                                   ? "Agreed" 
                                   : "Disparity" */
                         , candassessmentid=r.Id, orderid=ordr.Id
                         , taskdescription= "CV approved to send to client: Application No.:" + 
                              cand.ApplicationNo + ", Candidate: " + cand.FullName +
                              "forward to: " +  ordr.Customer.CustomerName + " against requirement " + 
                              ordr.OrderNo + "-" + i.SrNo + "-" + i.Category.Name +
                              ", Cleared to send by: " + r.AssessedByName + " on " + r.AssessedOn }
               ).ToListAsync();

               if (itemdetails.Count==0) {
                    dto.ErrorString="failed to retrieve appropriate data to register the CV Referrals";
                    return dto;
               }
               dbChanged=false;     

               foreach(var q in itemdetails)
               {
                    var cvref = await _context.CVRefs.Where(x => x.CandidateId == q.CandidateId && x.OrderItemId == q.OrderItemId).FirstOrDefaultAsync();
                    if (cvref != null) {          //logic error - candidateAssessment.Id has CvRefId == 0, but there is a record in CVRef.  
                         //this should never happen
                         var candass = await _context.CandidateAssessments.FindAsync(q.AsssessmentId);
                         if (candass != null) {
                              candass.CvRefId = cvref.Id;
                              _unitOfWork.Repository<CandidateAssessment>().Update(candass);
                              dbChanged=true;
                         }
                    } else {
                         //add the record for cVRef 
                         cvref =new CVRef(q.OrderItemId, q.CandidateId, q.CategoryId, q.OrderId, q.OrderNo,
                              q.CustomerName, q.CategoryName, q.CandidateId, q.Ecnr, q.ApplicationNo,
                              q.CandidateName, dateTimeNow, dateTimeNow
                              , q.Charges    //, (int)q.ChargesAgreed
                              , (int)q.HRExecId , 
                              q.CandidateAssessmentId);
                         _unitOfWork.Repository<CVRef>().Add(cvref);                      //1. create cvref record
                         
                         //task description to include in the TaskItem below
                         var candidatedescription = "Application " + q.ApplicationNo + " - " + q.CandidateName + " referred to " +
                                   q.CustomerName + " for " + q.OrderNo + "-" + q.SrNo + "-" + q.CategoryName + " on " + dateTimeNow;
                         
                         //mark Task with id as DcControllerAdminTaskId as completed
                         var t = await _context.Tasks.FindAsync(q.DocControllerAdminTaskId);
                         if(t != null) {
                              t.TaskStatus = "Completed";
                              t.CompletedOn=dateTimeNow;
                              _unitOfWork.Repository<ApplicationTask>().Update(t);             //2. Update Task, with Id as DocCtrollerADminTaskId
                              var taskitem = new TaskItem((int)EnumTaskType.SubmitCVToDocControllerAdmin, t.Id, dateTimeNow, "Completed",
                                   "Completed task: " + candidatedescription, cvref.OrderId, cvref.OrderItemId,
                                   q.OrderNo, loggedInUserDto.LoggedInEmployeeId, dateTimeNow.AddDays(2), q.CandidateId,
                                   0, DocControllerAdminId);
                              _unitOfWork.Repository<TaskItem>().Add(taskitem);
                         }

                    //4 - the CVReference is only updated in the DB - CREATE TASK in the name fo DocControllerAdmin
                    //   to physically send the email to the client;
                    //       THIS ALSO COMPOSES CV FORWARD MESSAGE
                         var CVFwdTask = await _context.Tasks.Where(x => x.OrderItemId == q.OrderItemId && 
                              x.CandidateId == q.CandidateId && x.TaskTypeId == (int)EnumTaskType.CVForwardToCustomers).FirstOrDefaultAsync();
                         if (CVFwdTask == null){
                              var itemdetail = itemdetails.Find(x => x.candassessmentid == q.AsssessmentId);
                              
                              CVFwdTask= new ApplicationTask((int)EnumTaskType.CVForwardToCustomers,
                                   dateTimeNow, loggedInUserDto.LoggedInEmployeeId, _docControllerAdminId, 
                                   itemdetail.orderid, itemdetail.OrderNo, itemdetail.OrderItemId, 
                                   itemdetail.taskdescription, dateTimeNow.AddDays(2), "Not Started", 
                                   itemdetail.CandidateId, q.AsssessmentId);
                              _unitOfWork.Repository<ApplicationTask>().Add(CVFwdTask);
                              //await _unitOfWork.Complete();                       
                         }


                    //5 - create task for DocController to follow up with client for selection
                         var taskForSelection = await _context.Tasks.Where(
                              x => x.TaskTypeId==(int)EnumTaskType.SelectionFollowupWithClient
                                   && x.AssignedToId==DocControllerAdminId
                                   && x.CandidateId  == q.CandidateId
                                   && x.OrderItemId == q.OrderItemId).FirstOrDefaultAsync();
                         if(taskForSelection==null) {
                              taskForSelection = new ApplicationTask((int)EnumTaskType.SelectionFollowupWithClient,
                              dateTimeNow, DocControllerAdminId, DocControllerAdminId, q.OrderId,
                              q.OrderNo, q.OrderItemId, "Follow up for selection of " + candidatedescription +
                              "referred on " + dateTimeNow.Date, dateTimeNow.AddDays(_TargetDays_FollowUpWithClientForSelection),
                              "Open", q.CandidateId, q.AsssessmentId);
                              
                              _unitOfWork.Repository<ApplicationTask>().Add(taskForSelection); //3. create new task in the name of DocControllerAdmin to follow up for selection.
                         } 

                         //CandidateAssessment.CVRef records will be updated later
                         dbChanged=true;
                         cvrefs.Add(cvref);
                    }
               }

               if (!dbChanged || cvrefs.Count ==0) {
                    dto.ErrorString = "No valid data available to register the CV Referrals";
                    return dto;
               }
               
               //6 - finally, compose CV Forward messages
               //save the cvrefs
               if(await _context.SaveChangesAsync() >0)  AssessmentIds = cvrefs.Select(x => x.CVReviewId).ToList();

               //update CVAssessments.cvrefid
               //q.candAssessment.TaskIdDocControllerAdmin = CVFwdTask.Id;
               //q.candAssessment.CvRefId = cvref.Id;
               //_unitOfWork.Repository<CandidateAssessment>().Update(q.candAssessment);    //5 - udate candidateAssessment.CVRefId field, so that
               var CVFwdTasks = await _context.Tasks.Where(x => 
                    cvrefs.Select(y => y.OrderItemId).ToList().Contains((int)x.OrderItemId) 
                    && cvrefs.Select(y => y.CandidateId).ToList().Contains((int)x.CandidateId)
                    && x.TaskTypeId==(int)EnumTaskType.CVForwardToCustomers).Select(x => new {x.Id, x.CandidateId, x.OrderItemId})
                    .ToListAsync();
                         
               var assessments = await _context.CandidateAssessments
                    .Where(x => itemdetails.Select(y => y.CandidateAssessmentId).ToList().Contains(x.Id)).ToListAsync();
                              //the CV is marked as Referred to the client, and removed from CVs to forward subsequently
               //var cvrefids = cvrefs.Select(x => x.Id).ToList();
               /*var cvrefT = await _context.CVRefs.Where(x =>             //cvrefT is sm as cvrfs.  so why cvrefT?
                    cvrefs.Select(x => x.CandidateId).ToList().Contains(x.CandidateId) 
                    && cvrefs.Select(x => x.OrderItemId).ToList().Contains(x.OrderItemId)
                    && x.ReferredOn==dateTimeNow).ToListAsync();
               */
               foreach(var item in assessments) {
                    var r = cvrefs.Where(x => x.CVReviewId == item.Id).FirstOrDefault();
                    item.CvRefId = r == null ? 0 : r.Id;
                    var tsk = CVFwdTasks.Where(x => x.CandidateId==item.CandidateId && x.OrderItemId==item.OrderItemId).FirstOrDefault();
                    item.TaskIdDocControllerAdmin = tsk==null ? 0 : (int)tsk?.Id;
                    _unitOfWork.Repository<CandidateAssessment>().Update(item);
               }

               var ct = await _context.SaveChangesAsync();      //this should hv happened after composemsgs, but any error in composeMsgs is forcing this to happen now

               var cvrefids = cvrefs.Select(x => x.Id).ToList();

               // prepare objects for composig msgs

               var cvrefWithCandidates = await (from refs in _context.CVRefs where cvrefids.Contains(refs.Id)
                    join cand in _context.Candidates on refs.CandidateId equals cand.Id
                    select new {CVRefId=refs.Id, CandidateName=cand.FullName, 
                         ApplicationNo=cand.ApplicationNo, OrderItemId=refs.OrderItemId,
                         PPNo=cand.PpNo})
                    .ToListAsync();
            
               var distinctOrderItemIds = cvrefWithCandidates.Select(x => x.OrderItemId).Distinct().ToList();

               var distinctOrderItems = await (from items in _context.OrderItems where distinctOrderItemIds.Contains(items.Id) 
                    join o in _context.Orders on items.OrderId equals o.Id 

                    select new {OrderItemId=items.Id, CustomerId=o.CustomerId, 
                         CustomerName = o.Customer.CustomerName, City = o.Customer.City,
                         OrderNo = o.OrderNo, OrderDated = o.OrderDate,
                         CategoryRef = o.OrderNo + "-" + items.SrNo + "-" + items.Category.Name,
                         ItemSrNo = items.SrNo
                    }).ToListAsync();

               var customerids = distinctOrderItems.Select(x => x.CustomerId).Distinct().ToList();
               var SelectedCustomerOfficials = await _context.CustomerOfficials
                    .Where(x => customerids.Contains(x.CustomerId) 
                    && !string.IsNullOrEmpty(x.Email)).ToListAsync();
            
               if(SelectedCustomerOfficials==null || SelectedCustomerOfficials.Count()==0) {
                    dto.ErrorString="Customer officials for the order Items not defined";
                    return dto;
                    }
               
               var CVReferredCounts = cvrefWithCandidates.Where(a => distinctOrderItems.Select(x => x.OrderItemId).ToList().Contains(a.OrderItemId))
                         .GroupBy(a => a.OrderItemId)
                         .Select(g => new { orderitemid= g.Key, refcount = g.Count() }).ToList();
               
               var cvfwddtos = new List<CVFwdMsgDto>();

               foreach(var cvref in cvrefWithCandidates) {
                    var orderitem = distinctOrderItems.Where(x => x.OrderItemId==cvref.OrderItemId).FirstOrDefault();
                    var Officials=SelectedCustomerOfficials.Where(x => x.CustomerId==orderitem.CustomerId).ToList();

                    var refCount = CVReferredCounts.Where(x => x.orderitemid==orderitem.OrderItemId).Select(x => x.refcount).FirstOrDefault();

                    var customerOfficial = new CustomerOfficial();
                    var officialToAssign = new CustomerOfficial();

                    if(Officials == null || Officials.Count()==0) {
                         //problem
                         continue;
                    } else if(Officials.Count ==1 ) {
                         officialToAssign = Officials.FirstOrDefault();
                    } else{
                         customerOfficial = Officials.Where(x => x.Divn?.ToLower()=="hr").FirstOrDefault();
                         if (customerOfficial != null) {
                              officialToAssign=customerOfficial;
                         } else {
                              customerOfficial = Officials.Where(x => x.Divn?.ToLower()=="admin").FirstOrDefault(); 
                              if(customerOfficial != null) {
                                   officialToAssign=customerOfficial;
                              } else {        //if official divn is not hr and not admin, then accept whatever is available
                                   officialToAssign=Officials.FirstOrDefault();
                              }
                         }
                    }

                    var cvfwddto = new CVFwdMsgDto{
                         CustomerId=orderitem.CustomerId,
                         CustomerName=orderitem.CustomerName,
                         City=orderitem.City,
                         OfficialId=officialToAssign.Id,
                         OfficialTitle=officialToAssign.Title,
                         OfficialName=officialToAssign.OfficialName,
                         OfficialUserId=officialToAssign.Id,
                         Designation=officialToAssign.Designation,
                         OfficialEmail=officialToAssign.Email,
                         OrderItemId=orderitem.OrderItemId,
                         OrderNo=orderitem.OrderNo,
                         OrderDated=orderitem.OrderDated,
                         ItemSrNo=orderitem.ItemSrNo,
                         CategoryName=orderitem.CustomerName,
                         ApplicationNo=cvref.ApplicationNo,
                         CandidateName=cvref.CandidateName,
                         PPNo=cvref.PPNo,
                         CumulativeSentSoFar=refCount
                    };
                    cvfwddtos.Add(cvfwddto);
               }
          //end of prepare

               var emailmsgs = _composeMsgAdmin.ComposeCVFwdMessagesToClient(cvfwddtos, loggedInUserDto);   //6 - compose email messages
               
               if(emailmsgs==null || emailmsgs.Count() ==0) {
                    dto.ErrorString = "CVs referred to client, but failed to compose cv forwarding messages";
                    return dto;
               }
               
               foreach(var msg in emailmsgs) {
                    _unitOfWork.Repository<EmailMessage>().Add(msg);
               }
               if (await _unitOfWork.Complete() == 0) {
                    dto.ErrorString = "Tasks for Document controller created, but messages were not composed";
                    return dto;
               }
               
               /*
               var filePaths = new List<string>();
               foreach (var msg in emailmsgs)
               {
                    if ((msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmail || 
                              msg.PostAction == (int)EnumPostTaskAction.ComposeAndSendEmailComposeAndSendSMS ))
                    _emailService.SendEmail(msg, filePaths);
               }
               */
               
               dto.emailMessages = emailmsgs;
               dto.CvRefIds=AssessmentIds;
               return dto;
               
               //todo - update the CVRef object with actual date cv sent - call back from email sent                    
          }

          public async Task<bool> DeleteReferral(CVRef cvref)
          {
               if (await _context.Deploys.Where(x => x.DeployCVRefId == cvref.Id).ToListAsync() != null)
               {
                    throw new System.Exception("The referral has related records in Deployments");
               }
               _unitOfWork.Repository<CVRef>().Delete(cvref);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<CVRefPendingDto>> GetCVsPendingReferralsToCustomer()
          {
               var qry = await (from rvw in _context.CVReviews
                    where rvw.CVRefId == 0 && rvw.DocControllerAdminTaskId > 0
                    join i in _context.OrderItems on rvw.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Candidates on rvw.CandidateId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    select new CVRefPendingDto
                    {
                         CandidateId = rvw.CandidateId,
                         CandidateDetails = "App No " + c.ApplicationNo + ", " + c.FullName + " PP No." + c.PpNo,
                         CategoryRef = o.OrderNo + "-" + i.SrNo,
                         ToBeReferredToCustomer = cust.CustomerName,
                         DateCVApprovedToForward =
                              i.NoReviewBySupervisor ? rvw.SubmittedByHRExecOn.Date
                              : rvw.HRMId == 0
                                   ? Convert.ToDateTime(rvw.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(rvw.HRMReviewedOn).Date,
                         CVApprovedByUsername = "",
                         NoReviewBySupervisor = i.NoReviewBySupervisor,
                         HRExecutiveId = (int)i.HrExecId,
                         HRSupId = (int)i.HrSupId,
                         HRMId = (int)i.HrmId
                    })
               .ToListAsync();

               foreach (var item in qry)
               {
                    item.CVApprovedByUsername = await _empService.GetEmployeeNameFromEmployeeId(
                         item.NoReviewBySupervisor ? item.HRExecutiveId : item.HRMId == 0 ? item.HRSupId : item.HRMId);
               }

               return qry;

          }

          public async Task<ICollection<CustomerReferralsPendingDto>> CustomerReferralsPending(int userId)
          {
               if (userId == 0)
               {
                    return await (from r in _context.CVReviews
                         where r.CVRefId == 0 && r.DocControllerAdminTaskId > 0
                         join i in _context.OrderItems on r.OrderItemId equals i.Id
                         join o in _context.Orders on i.OrderId equals o.Id
                         join cat in _context.Categories on i.CategoryId equals cat.Id
                         join cand in _context.Candidates on r.CandidateId equals cand.Id
                         join c in _context.Customers on o.CustomerId equals c.Id
                         select new CustomerReferralsPendingDto
                         {
                              CVReviewId = r.Id,
                              CandidateId = r.CandidateId,
                              ApplicationNo = cand.ApplicationNo,
                              CandidateName = cand.FullName,
                              OrderItemId = r.OrderItemId,
                              CategoryRef = o.OrderNo + "-" + i.SrNo,
                              CustomerName = c.CustomerName,
                              DocControllerAdminTaskId = (int)r.DocControllerAdminTaskId,
                              SentToDocControllerOn = r.NoReviewBySupervisor ? r.SubmittedByHRExecOn.Date :
                                   r.HRMId == 0 ? Convert.ToDateTime(r.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(r.HRMReviewedOn).Date
                         }).ToListAsync();
               }
               else
               {
                    return await (from r in _context.CVReviews
                         where r.CVRefId == 0 && r.DocControllerAdminTaskId > 0 && r.DocControllerAdminEmployeeId == userId
                         join i in _context.OrderItems on r.OrderItemId equals i.Id
                         join o in _context.Orders on i.OrderId equals o.Id
                         join cat in _context.Categories on i.CategoryId equals cat.Id
                         join cand in _context.Candidates on r.CandidateId equals cand.Id
                         join c in _context.Customers on o.CustomerId equals c.Id
                         select new CustomerReferralsPendingDto
                         {
                              CVReviewId = r.Id,
                              CandidateId = r.CandidateId,
                              ApplicationNo = cand.ApplicationNo,
                              CandidateName = cand.FullName,
                              OrderItemId = r.OrderItemId,
                              CategoryRef = o.OrderNo + "-" + i.SrNo,
                              CustomerName = c.CustomerName,
                              DocControllerAdminTaskId = (int)r.DocControllerAdminTaskId,
                              SentToDocControllerOn = r.NoReviewBySupervisor ? r.SubmittedByHRExecOn.Date :
                                   r.HRMId == 0 ? Convert.ToDateTime(r.ReviewedBySupOn).Date
                                   : Convert.ToDateTime(r.HRMReviewedOn).Date
                         }).ToListAsync();
               }
          }

          public async Task<Pagination<CVReferredDto>> GetCVReferredDto(CVRefParams refParams)
          {
               var qry =(from cvref in _context.CVRefs where cvref.OrderId==refParams.OrderId
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
                    join o in _context.Orders on cvref.OrderId equals o.Id 
                    join cat in _context.Categories on item.CategoryId equals cat.Id 
                    join cust in _context.Customers on o.CustomerId equals cust.Id 

               select new CVReferredDto {
                    CvRefId = cvref.Id,
                    CustomerName = cust.KnownAs,
                    OrderId = Convert.ToInt32(refParams.OrderId),
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = item.Id,
                    CategoryName = cat.Name,
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
                    CustomerId = o.CustomerId,
                    CandidateId = cvref.CandidateId,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    ReferredOn = cvref.ReferredOn,
                    ReferralDecision = cvref.RefStatus,
                    SelectedOn = cvref.RefStatusDate
               }).AsQueryable();

               if(refParams.OrderId > 0) qry.Where(x => x.OrderId==refParams.OrderId);
               if(refParams.CustomerId > 0) qry.Where(x => x.CustomerId==refParams.CustomerId);
               if(refParams.CVRefStatus.HasValue) qry.Where(x => x.ReferralDecision==refParams.CVRefStatus);

               qry = qry.OrderByDescending(x => x.OrderItemId).ThenByDescending(x => x.ReferredOn);

               var totalItems = await qry.CountAsync();

               var data = await qry.Skip((refParams.PageIndex-1)*refParams.PageSize).Take(refParams.PageSize) .ToListAsync();
               
               return new Pagination<CVReferredDto>(refParams.PageIndex, refParams.PageSize, totalItems, data);
          }

          public async Task<CVReferredDto> GetCVRefWithDeploys(int CVRefId)
          {

               var deps = new List<DeployDto>();
               
               var qry =  await (from cvref in _context.CVRefs where cvref.Id==CVRefId
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
                    join o in _context.Orders on cvref.OrderId equals o.Id 
                    join cat in _context.Categories on item.CategoryId equals cat.Id 
                    join cust in _context.Customers on o.CustomerId equals cust.Id 
                    

               select new CVReferredDto {
                    CvRefId = cvref.Id,
                    CustomerName = cust.KnownAs,
                    OrderId = o.Id,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = item.Id,
                    CategoryName = cat.Name,
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
                    CustomerId = o.CustomerId,
                    CandidateId = cvref.CandidateId,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    ReferredOn = cvref.ReferredOn,
                    ReferralDecision = cvref.RefStatus,
                    SelectedOn = cvref.RefStatusDate,
                    Deployments = deps
               }).FirstOrDefaultAsync();

               if (qry != null) {
               var deploys = (from d in _context.Deploys where d.DeployCVRefId==CVRefId
                    select new DeployDto(d.Id, d.DeployCVRefId, d.TransactionDate, Convert.ToInt32(d.Sequence), 
                    Convert.ToInt32(d.NextSequence), d.NextStageDate))
                    .AsQueryable();

                    deploys.OrderByDescending(x => x.TransactionDate);
               
                    var deployments = await deploys.ToListAsync();
                    qry.Deployments=deployments;

                    return qry;
               }

               return null;
          }
    
          public async Task<bool> ComposeSelDecisionReminderMessage(int CustomerId, LoggedInUserDto loggedinUser)
          {
               var qry =(from cvref in _context.CVRefs where cvref.RefStatus==(int)EnumCVRefStatus.Referred
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
                    join o in _context.Orders on cvref.OrderId equals o.Id where o.CustomerId == CustomerId
                    join cat in _context.Categories on item.CategoryId equals cat.Id 
                    join cust in _context.Customers on o.CustomerId equals cust.Id 

               select new CVReferredDto {
                    CvRefId = cvref.Id,
                    CustomerName = cust.KnownAs,
                    OrderId = o.Id,
                    SrNo = item.SrNo,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = item.Id,
                    CategoryName = cat.Name,
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
                    CustomerId = o.CustomerId,
                    CandidateId = cvref.CandidateId,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    ReferredOn = cvref.ReferredOn,
                    ReferralDecision = cvref.RefStatus,
                    SelectedOn = cvref.RefStatusDate
               }).AsQueryable();

               qry = qry.OrderByDescending(x => x.OrderItemId).ThenByDescending(x => x.ReferredOn);

               var refDtos = await qry.ToListAsync();

               if(refDtos==null || refDtos.Count==0) return false;

               var CustAndOfficial = await(from cust in _context.Customers.Where(x => x.Id == CustomerId)
                    join offs in _context.CustomerOfficials on cust.Id equals offs.CustomerId
                         where offs.Divn=="HR"
                    select new CustAndOfficialDto{
                         CustomerId = CustomerId, CustomerName = cust.CustomerName,
                         OfficialName = offs.OfficialName, OfficialDesignation = offs.Designation,
                         OfficialEmail = offs.Email
                    }).FirstOrDefaultAsync();

               var msg = _composeMsgAdmin.ComposeSelDecisionRemindersToClient(CustAndOfficial, refDtos, loggedinUser);

               _unitOfWork.Repository<EmailMessage>().Add(msg);

               return await _unitOfWork.Complete() > 0;

          }

    
    }

    
}