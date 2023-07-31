using System.Linq;
using core.Dtos;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using infra.Data;
using Microsoft.EntityFrameworkCore;


namespace infra.Services
{
     public class CandidateAssessmentService : ICandidateAssessmentService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ATSContext _context;
            private readonly IChecklistService _checklistHRService;
            private readonly ICommonServices _commonServices;
            private readonly ITaskService _taskService;
            private readonly ITaskControlledService _taskControlledService;
            public CandidateAssessmentService(IUnitOfWork unitOfWork, ATSContext context, 
                    IChecklistService checklistHRService, ICommonServices commonServices, 
                    ITaskService taskService, ITaskControlledService taskControlledService)
            {
                  this._taskControlledService = taskControlledService;
                _commonServices = commonServices;
                _checklistHRService = checklistHRService;
                _context = context;
                _unitOfWork = unitOfWork;
                _taskService=taskService;
            }


            public async Task<CandidateAssessmentWithErrorStringDto> AssessNewCandidate(bool requireInternalReview, int candidateId, int orderItemId, int loggedInEmployeeId)
            {
                
                var dto = new CandidateAssessmentWithErrorStringDto();

                var checklistHRid = await _checklistHRService.GetChecklistHRId(candidateId, orderItemId);
                if (checklistHRid == 0)  {
                    dto.ErrorString="Candidate not Checklisted";
                    return dto;      //No HR checklisting done
                }
                
                //check if the assessment already exists
                var candass = await (from ass in _context.CandidateAssessments where ass.CandidateId == candidateId && ass.OrderItemId == orderItemId
                    join e in _context.Employees on ass.AssessedById equals e.Id into emp
                    from empass in emp.DefaultIfEmpty()     //left join
                    select new {ass, empass.KnownAs} 
                    ).FirstOrDefaultAsync();
                
                if (candass != null) {
                    candass.ass.AssessedByName = candass.KnownAs;
                    dto.CandidateAssessment=candass.ass;
                    dto.ErrorString="Candidate already assessed for the same requirement";
                    return dto;
                }

                var candassessment = candass?.ass;
                //var loggedInUserId = await _context.Employees.Where(x => x.AppUserId == loggedInIdentityUserId).Select(x => x.Id).FirstOrDefaultAsync();

                var dateIssued = DateTime.Now;
                string knownas = await _commonServices.GetEmployeeNameFromEmployeeId(loggedInEmployeeId);

                if (!requireInternalReview) {        //the orderassessmentitems will not be set
                    candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInEmployeeId, knownas, dateIssued, 
                        false, checklistHRid, "Not Required");
                } else {
                    var itemassessment = await _context.OrderItemAssessments
                            .Where(x => x.OrderItemId == orderItemId)
                            .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                            .FirstOrDefaultAsync();
                    if (itemassessment == null) {
                        dto.ErrorString="Order Category Assessment parameters not defined";
                        return  dto;    //throw new System.Exception("Order Category assessment parameters not defined");                
                    }

                    var items = new List<CandidateAssessmentItem>();
                    foreach(var item in itemassessment.OrderItemAssessmentQs)
                    {
                        items.Add(new CandidateAssessmentItem(item.QuestionNo, item.Subject, item.Question, item.IsMandatory, item.MaxPoints));
                    }

                    candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInEmployeeId, knownas, dateIssued,true, checklistHRid, items);
                }

                _unitOfWork.Repository<CandidateAssessment>().Add(candassessment);

                if (await _unitOfWork.Complete() > 0) {

                    var qry = await GetOrderItemDetailForMessage(orderItemId, candidateId);

                    var task = await CreateNextApplicationTaskObject(candassessment.Id, orderItemId, 
                        candidateId, qry.ApplicationNo, qry.FullName, qry.NoReviewBySupervisor, qry.SrNo, 
                        (int)qry.HrSupId, (int)qry.HrmId, qry.FullName, qry.OrderId, qry.OrderNo, qry.OrderDate,
                        qry.CustomerName, qry.CustomerCity, candassessment.AssessedByName, candassessment.AssessedOn,
                        loggedInEmployeeId);

                    task.PostTaskAction=EnumPostTaskAction.OnlyComposeEmailMessage;
                    var msgs = await _taskControlledService.CreateNewTaskAndMsgs(task, loggedInEmployeeId);
                    candassessment.TaskIdDocControllerAdmin = task.Id;
                    dto.CandidateAssessment = candassessment;

                    _unitOfWork.Repository<CandidateAssessment>().Update(candassessment);
                    await _unitOfWork.Complete();
                    return dto;

                } else {
                    dto.ErrorString="Failed to update candidate assessment record";
                    return dto;
                }
            }

            public async Task<Pagination<CandidateAssessedDto>> GetShortlistedPaginated(CVRefParams cvrefParams)
            {

                var exceptList = new List<string>();
                exceptList.Add("Not Assessed");
                exceptList.Add("NotAssessed");
                exceptList.Add("Poor");
                exceptList.Add("Very Poor");

                
                var qry =(from ass in _context.CandidateAssessments 
                    where (ass.CvRefId == 0 && (ass.AssessResult == "Not Required" || !exceptList.Contains(ass.AssessResult))) 
                    join cv in _context.Candidates on ass.CandidateId equals cv.Id
                    //join agt in _context.Customers on cv.ReferredBy equals agt.Id
                    join item in _context.OrderItems on ass.OrderItemId equals item.Id
                    join o in _context.Orders on item.OrderId equals o.Id 
                    //join e in _context.Employees on ass.AssessedById equals e.Id
                    join checklist in _context.ChecklistHRs on new {a= item.Id, b = cv.Id} equals new {a = checklist.OrderItemId, b = checklist.CandidateId } into chklst
                    from chk in chklst.DefaultIfEmpty()
                    //orderby ass.OrderItemId
                    select new 
                         CandidateAssessedDto
                        {
                        Id = ass.Id, 
                        // AgentName = agt.KnownAs,
                        OrderItemId = ass.OrderItemId, 
                        CandidateId = cv.Id,
                        CvRefId= Convert.ToInt32(cv.ReferredBy),        //utilized cvrefid field to save referred by Id, since no other fld avlbl
                        AssessedById = ass.AssessedById, 
                        requireInternalReview = item.RequireInternalReview,
                        CustomerName = o.Customer.KnownAs,
                        AssessedOn = ass.AssessedOn, 
                        Remarks = ass.Remarks,
                        CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + item.Category.Name,
                        CandidateName = cv.FullName,
                        ApplicationNo = cv.ApplicationNo,
                        //AssessedByName = e.FirstName + " " + e.FamilyName,
                        AssessedResult = ass.AssessResult.ToString(),
                        Charges = chk == null ? "Undefined" 
                            : item.Charges == 0 ? "FOC"
                                : item.Charges == chk.ChargesAgreed ? "Agreed": chk.ExceptionApproved==true ? "Approved" : "Disparity"
                        
                    }).AsQueryable();
                    
                if (cvrefParams.CandidateId.HasValue) qry = qry.Where(x => x.CandidateId==cvrefParams.CandidateId);
                //if(cvrefParams.ProfessionId.HasValue) qry = qry.Where(x => x.ProfessionId==cvrefParams.ProfessionId);
                //if(cvrefParams.AgentId.HasValue) qry = qry.Where(x => x .AgentId==cvrefParams.AgentId);
                
                if(!string.IsNullOrEmpty(cvrefParams.Search)) qry = qry.Where(x => x.CandidateName.ToLower().Contains(cvrefParams.CandidateName));

                switch(cvrefParams.Sort.ToLower()) {
                   
                    case "customername":
                        qry = qry.OrderBy(x => x.CustomerName);
                        break;
                    case "customernamedesc":
                        qry=qry.OrderByDescending(x => x.CustomerName);
                        break;
                    default:
                        qry = qry.OrderBy(x => x.AssessedOn);
                        break;
                }
                
                var TotalCount = await qry.CountAsync();

                if (TotalCount==0) return null;
                
                var dto = await qry.Skip((cvrefParams.PageIndex-1)*cvrefParams.PageSize).Take(cvrefParams.PageSize).ToListAsync();
                
                //referredBy 
                var assessedids = dto.Select(x => x.AssessedById).Distinct().ToList();
                var emps = await _context.Employees.Where(x => assessedids.Contains(x.Id)).Select(x => new {x.Id, x.KnownAs}).ToListAsync();

                var agentids = dto.Select(x => x.CvRefId).Distinct().ToList();
                var assessedbyids = dto.Select(x => x.AssessedById).Distinct().ToList();
                agentids.AddRange(assessedbyids);
                var agentidname = await _context.Customers
                .Where(x => agentids.Contains(x.Id)).Select(x => new {x.Id, x.KnownAs}).ToListAsync();

                foreach(var d in dto) {
                    var known = agentidname.Find(x => x.Id==d.CvRefId);   // .Where(x => x.Id==d.CvRefId).FirstOrDefault();
                    if(known != null) {
                        d.AgentName = known.KnownAs;
                        d.CvRefId=0;
                    }
                    
                    if(d.AssessedById != 0) {
                        var assessedby=emps.Where(x => x.Id == d.AssessedById).FirstOrDefault();
                        d.AssessedByName=assessedby.KnownAs;
                    } 
                  
                }
                return new Pagination<CandidateAssessedDto>(cvrefParams.PageIndex, cvrefParams.PageSize, TotalCount, dto);
            }

            public async Task<ICollection<CandidateAssessedDto>> GetAssessedCandidatesApproved ()
            {
                var exceptList = new List<string>();
                exceptList.Add("Not Assessed");
                exceptList.Add("Poor");
                exceptList.Add("Very Poor");
                //if requieInternalReview then result assessed should not be from exceptList
                //and not referred
                var assessedDto = await (from ass in _context.CandidateAssessments 
                    where (ass.CvRefId == 0 && 
                        (!ass.requireInternalReview || (ass.requireInternalReview && !exceptList.Contains(ass.AssessResult)))) 
                    join cv in _context.Candidates on ass.CandidateId equals cv.Id
                    join cust in _context.Customers on cv.ReferredBy equals cust.Id
                    join item in _context.OrderItems on ass.OrderItemId equals item.Id
                    join o in _context.Orders on item.OrderId equals o.Id 
                    //join c in _context.Customers on o.CustomerId equals c.Id
                    //join cat in _context.Categories on item.CategoryId equals cat.Id
                    join e in _context.Employees on ass.AssessedById equals e.Id
                    join checklist in _context.ChecklistHRs on new {a= item.Id, b = cv.Id} equals new {a = checklist.OrderItemId, b = checklist.CandidateId } into chklst
                    from chk in chklst.DefaultIfEmpty()
                    orderby ass.OrderItemId
                    select new CandidateAssessedDto{
                        Id = ass.Id, 
                        OrderItemId = ass.OrderItemId, 
                        CandidateId = cv.Id,
                        AssessedById = ass.AssessedById, 
                        requireInternalReview = item.RequireInternalReview,
                        CustomerName = o.Customer.KnownAs,
                        AssessedOn = ass.AssessedOn, 
                        Remarks = ass.Remarks,
                        CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + item.Category.Name,
                        CandidateName = cv.FullName,
                        ApplicationNo = cv.ApplicationNo,
                        AgentName = cust.KnownAs,
                        CityName = cv.City,
                        AssessedByName = e.KnownAs,
                        AssessedResult = ass.AssessResult.ToString(),
                        Charges = chk == null ? "Undefined" 
                            : item.Charges == 0 ? "FOC"
                                : item.Charges == chk.ChargesAgreed ? "Agreed": chk.ExceptionApproved==true ? "Approved" : "Disparity"
                    }).ToListAsync();
                
                return assessedDto;
            }

            public async Task<CandidateAssessment> GetNewAssessmentObject(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId)
            {
                    var checklisthrid = await _checklistHRService.GetChecklistHRId(candidateId, orderItemId);
                    if (checklisthrid == 0) return null;

                    //check if the assessment already exists
                    var candassessment = new CandidateAssessment();
                    var knownas = await _commonServices.GetEmployeeNameFromEmployeeId(loggedInIdentityUserId);
                    var dateIssued = DateTime.Now;
                    if (!requireInternalReview) {        //the orderassessmentitems will not be set
                        candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInIdentityUserId, knownas, dateIssued,false, checklisthrid, "Not Required");
                    } else {
                        var itemassessment = await _context.OrderItemAssessments
                                .Where(x => x.OrderItemId == orderItemId)
                                .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                                .FirstOrDefaultAsync();
                        if (itemassessment == null) return null;    //throw new System.Exception("Order Category assessment parameters not defined");                

                        var items = new List<CandidateAssessmentItem>();
                        foreach(var item in itemassessment.OrderItemAssessmentQs)
                        {
                            items.Add(new CandidateAssessmentItem(item.QuestionNo, item.Subject, item.Question, item.IsMandatory, item.MaxPoints));
                        }
        
                        candassessment = new CandidateAssessment(candidateId, orderItemId, loggedInIdentityUserId, knownas, dateIssued,true, checklisthrid, items);
                    }
                    
                    return candassessment;
            }

            public async Task<bool> DeleteCandidateAssessment(int assessmentid)
            {
                var candidateAssessment = await _context.CandidateAssessments.FindAsync(assessmentid);
                if (candidateAssessment == null) return false;
                if(candidateAssessment.TaskIdDocControllerAdmin > 0) {
                    var task = await _context.Tasks.FindAsync(candidateAssessment.TaskIdDocControllerAdmin);
                    _unitOfWork.Repository<ApplicationTask>().Delete(task);
                } else {
                        var task = await _context.Tasks.Where(x => 
                            x.CandidateId == candidateAssessment.CandidateId &&
                            x.OrderItemId == candidateAssessment.OrderItemId)
                            .FirstOrDefaultAsync();
                        if (task != null) _unitOfWork.Repository<ApplicationTask>().Delete(task);
                }
                
                _unitOfWork.Repository<CandidateAssessment>().Delete(candidateAssessment);
                return await _unitOfWork.Complete() > 0;
                
            }
            public async Task<bool> DeleteCandidateAssessmentItem(CandidateAssessmentItem assessmentItem)
            {
                _unitOfWork.Repository<CandidateAssessmentItem>().Delete(assessmentItem);
                return await _unitOfWork.Complete() > 0;
            }

            //once a candidate assessment is approved, escalate to next stage
            //the CV is submitted to either HRSup, HRM or Doc Controller, depending upon order item definitions
            public async Task<MessagesDto> EditCandidateAssessment(CandidateAssessment candidateAssessment, int loggedinEmployeeId, string employeeName)
        {
            var msgsDto = new MessagesDto();

            //update assessmentResult field based upon totals of items
                var assessResult = "";
                var totalPoints=0;
                if (candidateAssessment.CandidateAssessmentItems.Count > 0){
                    totalPoints=100* candidateAssessment.CandidateAssessmentItems.Sum(x => x.Points) / candidateAssessment.CandidateAssessmentItems.Sum(x => x.MaxPoints);
                    switch(totalPoints)
                    {
                            case >=90:
                                assessResult = "Excellent";
                                break;
                            case >=80:
                                assessResult = "Very Good";
                                break;
                            case >=70:
                                assessResult = "Gpod";
                                break;
                            case >=50:
                                assessResult = "Poor";
                                break;
                            default:
                                assessResult = "Very Poor";
                                break;
                    }
                    candidateAssessment.AssessResult=assessResult;
                }
                
                
            _unitOfWork.Repository<CandidateAssessment>().Update(candidateAssessment);

                foreach(var item in candidateAssessment.CandidateAssessmentItems)
                {
                    _unitOfWork.Repository<CandidateAssessmentItem>().Update(item); 
                }

                if (await _unitOfWork.Complete() == 0) {
                    msgsDto.ErrorString = "Failed to update Candidate Assessment records";
                    return msgsDto;
                }
                
                var qry = await GetOrderItemDetailForMessage(candidateAssessment.OrderItemId, candidateAssessment.CandidateId);
                    if (qry==null) {
                        msgsDto.ErrorString="Failed to retrieve order item details - this is severe in nature, please inform your developers";
                        return msgsDto;     //this should never happen
                    }
                var taskdescription="Application No.:" + qry.ApplicationNo + ", Candidate: " + qry.FullName + ", assessed for " + qry.CustomerName +
                        ", " + qry.CustomerCity + "against Order No.:" + qry.OrderNo + " dated " + qry.OrderDate + ", category ref: " + qry.SrNo +
                        "-" + qry.CategoryName + ", assessed by " + candidateAssessment.AssessedByName + " on " + candidateAssessment.AssessedOn;

                var task = await _context.Tasks.Where(x =>
                        x.OrderItemId == candidateAssessment.OrderItemId && x.CandidateId == candidateAssessment.CandidateId &&
                        x.TaskTypeId == (int)EnumTaskType.CVForwardToCustomers && x.AssignedToId == 4
                    ).FirstOrDefaultAsync();

                if (task!= null) {
                        if (task.CVReviewId != candidateAssessment.Id) {
                        task.CVReviewId=candidateAssessment.Id;
                        task.TaskDescription=taskdescription;   
                        msgsDto = await _taskControlledService.EditApplicationTask(task, loggedinEmployeeId, employeeName); //consider to do all db updates here instead
                        } 
                    
                } else {
                    task = await CreateNextApplicationTaskObject(candidateAssessment.Id, candidateAssessment.OrderItemId, 
                        candidateAssessment.CandidateId, qry.ApplicationNo, qry.FullName, qry.NoReviewBySupervisor, qry.SrNo, 
                        (int)qry.HrSupId, (int)qry.HrmId, qry.FullName, qry.OrderId, qry.OrderNo, qry.OrderDate,
                        qry.CustomerName, qry.CustomerCity, candidateAssessment.AssessedByName, candidateAssessment.AssessedOn,
                        loggedinEmployeeId);
                    
                    msgsDto = await _taskControlledService.CreateNewTaskAndMsgs(task, loggedinEmployeeId);    //*TODO* this saves the data. consider all db changes here in one _unitOFWork.Complete()
                
                }

                //create tasks for DocumentControllerAdmin;
                
                
                if(task != null) candidateAssessment.TaskIdDocControllerAdmin = task.Id;
                _unitOfWork.Repository<CandidateAssessment>().Update(candidateAssessment);
                await _unitOfWork.Complete();
                if (msgsDto==null || msgsDto?.emailMessages?.Count ==0) {
                    msgsDto.ErrorString="Failed to create tasks for the document administrator";
                    return msgsDto;
                }
                
                return msgsDto;
        }

            private async Task<OrderItemDetailForMsgDto> GetOrderItemDetailForMessage(int OrderItemId, int CandidateId)
            {
                var qry = await (from i in _context.OrderItems where i.Id == OrderItemId
                    from cv in _context.Candidates where cv.Id == CandidateId
                    join o in _context.Orders on i.OrderId equals o.Id
                    // join c in _context.Customers on o.CustomerId equals c.Id
                    // join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new  OrderItemDetailForMsgDto{
                        SrNo = i.SrNo,  
                        HrSupId= i.HrSupId==null ? 0 : (int)i.HrSupId, 
                        HrmId= i.HrmId==null ? 0 : (int)i.HrmId, 
                        CategoryName=i.Category.Name, 
                        OrderId=o.Id, 
                        OrderNo=o.OrderNo, 
                        OrderDate=o.OrderDate, 
                        CustomerName=o.Customer.CustomerName, 
                        CustomerCity= o.Customer.City, 
                        ApplicationNo=cv.ApplicationNo, 
                        FullName=cv.FullName, 
                        NoReviewBySupervisor=i.NoReviewBySupervisor})
                    .FirstOrDefaultAsync();
                
                return qry;
            }

            private async Task<ApplicationTask> CreateNextApplicationTaskObject(int candidateassessmentid, int orderitemid,
                int candidateid,int applicationno, string candidatename, bool noreviewbysup, int srno, int hrsupid, int hrmid, 
                string catname, int orderid, int orderno, DateTime orderdate, string customername, string cityname, 
                string assessedbyname, DateTime assessedon, int taskownerid)
        {
                //create tasks for DocumentControllerAdmin;
                string EnvAssignedToId="";
                string EnvPeriodInDays="";
                int intDays = 0;
                int intAssignedToId=0;
                string taskdescription="";
                string assignedtoname="";

                string appdetails="Application No.:" + applicationno +
                    ", Candidate: " + candidatename + ", assessed for " + customername + ", " + cityname +
                    "against Order No.:" + orderno + " dated " + orderdate + ", category ref: " + srno + "-" + catname +
                    ", assessed by " + assessedbyname + " on " + assessedon;

                if (noreviewbysup || (hrsupid ==0 && hrmid ==0)) {
                        EnvAssignedToId="4";  //Environment["EmpDocControllerAdminId"]; // .GetEnvironmentVariable("EmpDocControllerAdminId");
                        EnvPeriodInDays = "1";    //Environment.GetEnvironmentVariable("TargetDays_DocControllerToFwdCV");

                        taskdescription= appdetails + " -  is approved for forwarding to client";
                } else if (hrsupid > 0) {
                        intAssignedToId=hrsupid;
                        EnvPeriodInDays = "2";    //Environment.GetEnvironmentVariable("TargetDays_HRSupToReviewCV");
                        
                        taskdescription= appdetails + " - is submitted to HR Supervisor for review";
                } else if (hrmid > 0) {
                        intAssignedToId=hrmid;
                        EnvPeriodInDays = "2";    // Environment.GetEnvironmentVariable("TargetDays_HRMToReviewCV");
                        
                        taskdescription= appdetails + " - is submitted to HR Manager for review";
                }
                    
                if (!string.IsNullOrEmpty(EnvAssignedToId)) intAssignedToId=Convert.ToInt32(EnvAssignedToId);
                if (!string.IsNullOrEmpty(EnvPeriodInDays)) intDays=Convert.ToInt32(EnvPeriodInDays);
                if(intAssignedToId>0)assignedtoname = await _commonServices.GetEmployeeNameFromEmployeeId(intAssignedToId);


                var newtask = new ApplicationTask((int)EnumTaskType.CVForwardToCustomers, 
                    DateTime.Now, taskownerid, intAssignedToId, orderid, orderno, orderitemid, taskdescription, 
                    DateTime.Now.AddDays(intDays), "Not Started", candidateid, candidateassessmentid);
                
                return newtask;

        }

            public async Task<CandidateAssessment> GetCandidateAssessment(int candidateId, int orderItemId)
            {
                var assess = await _context.CandidateAssessments
                .Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
                .Include(x => x.CandidateAssessmentItems.OrderBy(x => x.QuestionNo))
                .FirstOrDefaultAsync();
            
                return assess;
            }
        
            private async Task<CandidateAssessment> GetOrCreateCandidateAsessment(int candidateid, int orderitemid, int loggedInEmployeeId)
            {
                var one = await _context.CandidateAssessments
                    .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderitemid)
                    .Include(x => x.CandidateAssessmentItems)
                    .FirstOrDefaultAsync();
                
                if(one==null) {
                    var requireInternalReview = await _context.OrderItems.Where(x => x.Id==orderitemid)
                        .Select(x => x.RequireInternalReview).FirstOrDefaultAsync();
                    var assessmentWithErrObj = await AssessNewCandidate(requireInternalReview, candidateid, orderitemid, loggedInEmployeeId);
                    if(string.IsNullOrEmpty(assessmentWithErrObj.ErrorString)) {
                        one = assessmentWithErrObj.CandidateAssessment;
                    } else {
                        one = null;
                    }
                }

                return one;
                }
            public async Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentAndChecklist(int candidateId, int orderItemId, int loggedInEmployeeId)
            {
            
                var one = await GetOrCreateCandidateAsessment(candidateId, orderItemId, loggedInEmployeeId);
                
                if(one==null) return null;
                
                //if CandidateAssessmentItems.count==0, then insert relevant items
                if(one.CandidateAssessmentItems==null || one.CandidateAssessmentItems.Count==0) {
                    var items = await _context.OrderItemAssessmentQs.Where(x => x.OrderItemId==orderItemId)
                        .OrderBy(x => x.QuestionNo).ToListAsync();
                    if(items==null ||items.Count==0) return null;
                    var assessmentItems = new List<CandidateAssessmentItem>();

                    foreach(var item in items){
                        var assessmentItem = new CandidateAssessmentItem(one.Id, item.QuestionNo, item.Subject, 
                            item.Question, item.IsMandatory, item.MaxPoints,0,"");
                        assessmentItems.Add(assessmentItem);
                        _context.Entry(assessmentItem).State=EntityState.Added;
                    }
                    
                    one.CandidateAssessmentItems= assessmentItems;
                    
                    await _context.SaveChangesAsync();
                }
                
                var temp = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateId && x.OrderItemId==orderItemId)
                    .Include(x => x.ChecklistHRItems).FirstOrDefaultAsync();
                if (temp == null) {
                    temp = await _checklistHRService.AddNewChecklistHR(candidateId, orderItemId, loggedInEmployeeId);
                }
            
                var twof = await (from chklst in  _context.ChecklistHRs 
                            where chklst.CandidateId == candidateId && chklst.OrderItemId == orderItemId 
                        join item in _context.OrderItems on chklst.OrderItemId equals item.Id
                        join cat in _context.Categories on item.CategoryId equals cat.Id
                        join cv in _context.Candidates on chklst.CandidateId equals cv.Id
                        join o in _context.Orders on item.OrderId equals o.Id
                        join c in _context.Customers on o.CustomerId equals c.Id
                        join e in _context.Employees on chklst.UserId equals e.Id into emp
                        from em in emp.DefaultIfEmpty()
                
                        select new ChecklistHRDto {
                            CandidateId = candidateId,
                            Id = chklst.Id, 
                            ApplicationNo = cv.ApplicationNo,
                            OrderItemId = orderItemId,
                            CandidateName = string.IsNullOrEmpty(cv.FullName) ? "" : cv.FullName,
                            
                            UserLoggedId = chklst.UserId, 
                            UserLoggedName = em.KnownAs,
                            CheckedOn = chklst.CheckedOn, 
                            CategoryRef = o.OrderNo + "-" + "-" + cat.Name,
                            OrderRef = c.CustomerName,
                            HrExecComments = string.IsNullOrEmpty(chklst.HrExecComments) ? "" : chklst.HrExecComments,
                            Charges = item.Charges, ChargesAgreed = chklst.ChargesAgreed,
                            ExceptionApproved = chklst.ExceptionApproved, ExceptionApprovedBy = chklst.ExceptionApprovedBy,
                            ExceptionApprovedOn = chklst.ExceptionApprovedOn,
                            ChecklistedOk = chklst.ChecklistedOk,
                            ChecklistHRItems = chklst.ChecklistHRItems
                        }
                    ).FirstOrDefaultAsync();
                
                return new CandidateAssessmentAndChecklistDto(one, twof);

            }

            public async Task<ICollection<AssessmentsOfACandidateIdDto>> GetCandidateAssessmentHeaders(int candidateid)
            {
                
                var qry = await (from ass in _context.CandidateAssessments where ass.CandidateId==candidateid
                    select new {ChecklistedOn = ass.AssessedOn, AssessedByName = ass.AssessedByName}).ToListAsync();
                var assessments = await _context.CandidateAssessments.Where(x => x.CandidateId == candidateid)
                    .Select(x => new {x.AssessedOn, x.AssessedByName, x.CandidateId, x.OrderItemId}).ToListAsync();
                
                var details = await (from checklist in _context.ChecklistHRs where checklist.CandidateId==candidateid
                    join emp in _context.Employees on checklist.UserId equals emp.Id
                    join i in _context.OrderItems on checklist.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    select new {
                        CandidateId = checklist.CandidateId,
                        OrderItemId = checklist.OrderItemId,
                        ChecklistedByName=emp.KnownAs,
                        ChecklistedOn=checklist.CheckedOn,
                        CustomerName = o.Customer.KnownAs, 
                        CategoryName = i.Category.Name, 
                        Knownas = emp.KnownAs,
                        CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + i.Category.Name
                    }).ToListAsync();
                
                var lst = new List<AssessmentsOfACandidateIdDto>();
                foreach(var ass in assessments) {
                    var detail = details.FirstOrDefault(x => x.CandidateId == ass.CandidateId && x.OrderItemId == ass.OrderItemId);
                    if (detail != null) {
                        lst.Add(new AssessmentsOfACandidateIdDto {
                            ChecklistedByName=detail.Knownas,
                            ChecklistedOn=detail.ChecklistedOn,
                            CustomerName = detail.CustomerName, 
                            CategoryName = detail.CategoryName, 
                            CategoryRef = detail.CategoryRef,
                            AssessedOn = ass.AssessedOn,
                            AssessedByName = ass.AssessedByName
                        });
                    }
                }       

                return lst;
            }

            public async Task<ChecklistHRDto> CreateChecklist(int candidateid, int orderitemid, int loggedInEmpId)
            {
                var checklistid=0;

                var checklist = await _context.ChecklistHRs.Include(x => x.ChecklistHRItems)    
                    .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderitemid)
                    .FirstOrDefaultAsync();

                var checklistitems = new List<ChecklistHRItem>();

                if(checklist==null || checklist.ChecklistHRItems.Count==0 ) {
                    var checklistdata = await _context.ChecklistHRDatas.OrderBy(x => x.SrNo).ToListAsync();
                    foreach(var data in checklistdata)
                    {
                        checklistitems.Add(new ChecklistHRItem(data.SrNo, data.Parameter));
                    }
                }
                if(checklist==null) {
                    var newchecklist = new ChecklistHR(candidateid, orderitemid, loggedInEmpId, DateTime.Now, checklistitems);
                    //_context.Entry(checklist).State=EntityState.Added;
                    _context.ChecklistHRs.Add(newchecklist);
                    await _context.SaveChangesAsync();
                    checklistid = checklist.Id==0 ? newchecklist.Id : checklist.Id;
                }
                
                if(checklist.ChecklistHRItems.Count==0 && checklistitems.Count>0) {
                    checklist.ChecklistHRItems=checklistitems;
                    _context.Entry(checklist).State=EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                //prepare for ChecklistHRDto
                var qry = await _context.Candidates.Where(x => x.Id==candidateid)
                    .Select(x => new {candidatename = x.FullName, applicationno=x.ApplicationNo, shortname=x.KnownAs})
                    .FirstOrDefaultAsync();
                if (qry==null) return null;

                var item = await (from oitem in _context.OrderItems where oitem.Id==orderitemid
                    join cat in _context.Categories on oitem.CategoryId equals cat.Id
                    join o in _context.Orders on oitem.OrderId equals o.Id 
                    select new {categoryref = o.OrderNo + "-" + oitem.SrNo + "-" + cat.Name, 
                    orderref = o.OrderNo + "-" + oitem.SrNo, charges=oitem.Charges}
                    ).FirstOrDefaultAsync();

                if(item==null) return null;

                checklistid = checklist.Id ==0 ? checklistid : checklist.Id;

                var dto = new ChecklistHRDto(checklistid, candidateid, qry.applicationno, qry.candidatename, orderitemid, item.categoryref, 
                    item.orderref, loggedInEmpId, qry.shortname, item.charges, checklist.ChecklistHRItems);
                
                return dto;
            }
    }
}