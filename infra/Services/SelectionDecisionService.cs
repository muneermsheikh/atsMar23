using AutoMapper;
using core.Dtos;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Interfaces;
using core.Params;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class SelectionDecisionService : ISelectionDecisionService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IDeployService _deployService;
          private readonly ATSContext _context;
          private readonly IComposeMessages _composeMessages;
          private readonly IEmailService _emailService;
          private readonly IMapper _mapper;
          private readonly IComposeMessagesForAdmin _composeMsgForAdmin;
          private readonly int HRSupEmpTaskId=12;
          private readonly string m_SenderEmailAddresss="hr@afreenitl.in";
          public SelectionDecisionService(IUnitOfWork unitOfWork, ATSContext context, ICommonServices commonServices, IComposeMessagesForAdmin composeMsgForAdmin,
          IComposeMessages composeMessages, IDeployService deployService, IEmailService emailService, IMapper mapper)
          {
               _composeMsgForAdmin = composeMsgForAdmin;
               _mapper = mapper;
               _emailService = emailService;
               _composeMessages = composeMessages;
               _context = context;
               _deployService = deployService;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
          }

          public async Task<bool> DeleteSelection(int id)
          {
               var selectionDecision = await _context.SelectionDecisions.FindAsync(id);
               if (selectionDecision == null) return false;
               
               _unitOfWork.Repository<SelectionDecision>().Delete(selectionDecision);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditSelection(SelectionDecision selectionDecision)
          {
               _unitOfWork.Repository<SelectionDecision>().Update(selectionDecision);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<Pagination<SelectionDecision>> GetSelectionDecisions(SelDecisionSpecParams specParams)
          {
               var spec = new SelDecisionSpecs(specParams);
               var specCount = new SelDecisionForCountSpecs(specParams);
               var decisions = await _unitOfWork.Repository<SelectionDecision>().ListAsync(spec);
               var ct = await _unitOfWork.Repository<SelectionDecision>().CountAsync(specCount);

               return new Pagination<SelectionDecision>(specParams.PageIndex, specParams.PageSize, ct, decisions);
          }

          //for each SeldecisionToAddDto,
          //a1 - create employment record,
          //a2 - CREATE THE MAIN SelectionDcision record
          //a3 - Create Deployment Record
          //a4 - Create task in the name of the HR Exec Id (defined in OrderItem) or HR Supervior of HRExec Id undefined
          //a5 - Update CVRef record with deployment stage information

          //B1 - for all flows - selected and rejected, update CVRef with refStatus and RefStatusDate
          //b2 - update doc controller task for selection , whether seleced or rejected, teh task is over
          public async Task<SelectionMsgsAndEmploymentsDto> RegisterSelections(SelDecisionsToAddParams selParams, int loggedInEmployeeId, string loggedInUserName)
          {
               DateTime dateTimeNow = DateTime.Now;
               var selDto = selParams.SelDecisionsToAddDto;

               var seldecisions = new List<SelectionDecision>();
               var seldecision = new SelectionDecision();

               var empDtos = new List<EmploymentDto>();
               var cvrefids = new List<int>();

               foreach(var s in selDto)
               {
                    cvrefids.Add(s.CVRefId);
               }
               
               
               var selectedMsgsDto = new List<SelectionMessageDto>();
               var rejectedMsgsDto = new List<SelectionMessageDto>();


               //var cvrefs = await _context.CVRefs.Where(x => cvrefids.Contains(x.Id)).ToListAsync();
               var recAffected=0;
               var dtls = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join empE in _context.Employees on item.HrExecId equals empE.Id into empExecutive
                    from empExec in empExecutive.DefaultIfEmpty()
                    select new {item.CategoryName, cvref, item.OrderNo, item.Id, o.Customer.CustomerName,  o.OrderDate,
                         HRExecKnownAs = empExec == null ? "" : empExec.KnownAs, HRExecEmail = empExec == null ? "" : empExec.Email
                    }).ToListAsync();

               //detals is the object that contains all details required:
               //   --   to update CVRef with selection/rejection values
               //   --   to create employment data if selected
               //   --   to create Selectionecision data 
               //   --   to complete tasks that were created in CV Forwarding module, to follow up with client for selections
               //   --   to create tasks for processing team if selected
               //   --   to compose selection of rejection messages to candidates
               var details = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                    //join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    //join c in _context.Customers on o.CustomerId equals c.Id
                    join employeeExec in _context.Employees on item.HrExecId equals employeeExec.Id into ExecEmployees
                    from empExec in ExecEmployees.DefaultIfEmpty()
                    join employeeSup in _context.Employees on item.HrSupId equals employeeSup.Id into SupEmployees
                    from empSup in SupEmployees.DefaultIfEmpty()
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                    select new {
                         cvref, CVRefId=cvref.Id, CandidateName = cv.FullName, CandidateTitle="Mr.",     //**TODO*8 Add CandidateTitle to candidate
                         CandidateEmail = cv.Email, CandidateId=cv.Id, CandidateGender=cv.Gender, CandidateKnownAs=cv.KnownAs,
                         CategoryRef=o.OrderNo + "-" + item.SrNo + item.Category.Name,
                         CustomerName = o.Customer.CustomerName,  CustomerCity=o.Customer.City,
                         ApplicationNo = cv.ApplicationNo, Charges=item.Charges,
                         OrderItemId = cvref.OrderItemId, 
                         CategoryId = item.CategoryId, CategoryName = item.Category.Name,
                         OrderId=o.Id, OrderNo = o.OrderNo, 
                         
                         HRExecId = empExec==null ? 0 : empExec.Id, 
                         HRExecName = empExec==null ? "" : empExec.KnownAs, 
                         HRExecEmail = empExec == null ? "" : empExec.Email, 
                         HRExecGender = empExec == null ? "" :  empExec.Gender,

                         HRSupId = empSup == null ? 0 : empSup.Id, 
                         HRSupName = empSup == null ? "" : empSup.KnownAs, 
                         HRSupEmai = empSup == null ? "" : empSup.Email, 
                         HRSupGender = empSup == null ? "" : empSup.Gender
                    }).ToListAsync();
               
               foreach(var dtl in details) {
                    var cvref = dtl.cvref;
                    if(cvref.ApplicationNo==0) cvref.ApplicationNo=dtl.ApplicationNo;
                    if(string.IsNullOrEmpty(cvref.CandidateName)) cvref.CandidateName=dtl.CandidateName;
                    if(string.IsNullOrEmpty(cvref.CategoryName)) cvref.CandidateName=dtl.CategoryRef;
                    if(string.IsNullOrEmpty(cvref.CustomerName)) cvref.CustomerName = dtl.CustomerName;
                    if(cvref.Charges==0) cvref.Charges = dtl.Charges;
                    if(cvref.OrderItemId==0) cvref.OrderItemId = dtl.OrderItemId;
                    if(cvref.OrderId==0) cvref.OrderId = dtl.OrderId;
                    if(cvref.OrderNo==0) cvref.OrderNo = dtl.OrderNo;
                    if(cvref.CategoryId==0) cvref.CategoryId = dtl.CategoryId;
               }
               
               var employments = new List<Employment>();

               foreach (var s in selDto)
               {
                    var cvref = details.Where(x => x.cvref.Id== s.CVRefId)
                         .Select(x => new {x.CVRefId, x.cvref, x.OrderItemId,
                              x.OrderId, x.OrderNo, x.CustomerName,
                              x.CategoryId, x.CategoryName,
                              x.ApplicationNo, x.CandidateName, x.CandidateId, x.CandidateGender, x.CandidateEmail, 
                                   x.CandidateKnownAs, x.CandidateTitle, x.CustomerCity,
                              x.HRExecId, x.HRExecGender, x.HRExecName, x.HRExecEmail,
                              x.HRSupId, x.HRSupName, x.HRSupEmai, x.HRSupGender
                         }).FirstOrDefault();

                    if(cvref==null)  continue;        //save the error

                    var emp=new Employment();
                    
                    //A1 - create employment record
                    if(s.SelectionStatusId==(int)EnumCVRefStatus.Selected) {
                         var salCurrency = await getSalaryCurrency(cvref.OrderItemId);
                              emp = await _context.Employments.Where(x => x.CVRefId == cvref.CVRefId).FirstOrDefaultAsync();
                              if(emp==null) {
                                   emp = new Employment(cvref.CVRefId, s.DecisionDate,salCurrency,0,24,false,0,false,0,false,0,0,21,24,0 );
                                   _unitOfWork.Repository<Employment>().Add(emp);
                                   recAffected++;
                                   employments.Add(emp);
                              }
                         
                         //A2 - CREATE THE MAIN SelectionDcision record
                         seldecision = new SelectionDecision( cvref.CategoryId, cvref.CategoryName, cvref.OrderId, cvref.OrderNo, 
                              cvref.CustomerName, cvref.ApplicationNo, cvref.CandidateName, s.DecisionDate, 
                              s.SelectionStatusId, s.Remarks, cvref.cvref, emp);

                         seldecisions.Add(seldecision);

                         _unitOfWork.Repository<SelectionDecision>().Add(seldecision);
                    }

                    var selectedMsgDto = new SelectionMessageDto(
                         cvref.CustomerName, cvref.CustomerCity, cvref.OrderNo, cvref.CategoryName, 
                              s.SelectionStatusId==(int)EnumCVRefStatus.Selected ? emp : null, 
                              s.SelectionStatusId==(int)EnumCVRefStatus.Selected ? seldecision : null, 
                         cvref.ApplicationNo, cvref.CandidateId, cvref.CandidateTitle, cvref.CandidateName,
                         cvref.CandidateGender, cvref.CandidateEmail, cvref.CandidateKnownAs, cvref.HRExecId,
                         cvref.HRExecName, cvref.HRExecEmail, cvref.HRSupId, cvref.HRSupName, cvref.HRSupEmai
                    );

                    //selectedMsgsDto.Add(selectedMsgDto);

                    recAffected++;
                    
                    if (s.SelectionStatusId == (int)EnumCVRefStatus.Selected)
                    {
                         selectedMsgsDto.Add(selectedMsgDto);
                         //A3 - Create Deployment Record
                         var nextStage = await _context.DeployStages.Where(x => x.Id==(int)EnumDeployStatus.Selected).FirstOrDefaultAsync();
                         
                         var deployTrans = new Deploy {
                              CVRefId=cvref.CVRefId, TransactionDate=s.DecisionDate, StageId=EnumDeployStatus.Selected, 
                              NextStageId=(EnumDeployStatus)nextStage.NextDeployStageSequence, 
                              NextEstimatedStageDate=s.DecisionDate.AddDays(nextStage.EstimatedDaysToCompleteThisStage),
                              CVRef=cvref.cvref
                         };

                         _unitOfWork.Repository<Deploy>().Add(deployTrans);     
                         recAffected++;

                         //A4 - Create task in the name of the HR Exec Id (defined in OrderItem) or HR Supervior of HRExec Id undefined
                         var HRExecTask = await _context.Tasks.Where(x => x.OrderItemId == cvref.OrderItemId && 
                              x.CandidateId == cvref.CandidateId && x.AssignedToId == (cvref.HRExecId == 0 ? HRSupEmpTaskId : cvref.HRExecId)
                              && x.TaskTypeId == (int)EnumTaskType.OfferLetterAcceptance).FirstOrDefaultAsync();
                         if (HRExecTask==null) {
                              HRExecTask = new ApplicationTask((int)EnumTaskType.OfferLetterAcceptance, dateTimeNow,
                                   loggedInEmployeeId, cvref.HRExecId == 0 ? HRSupEmpTaskId : cvref.HRExecId, cvref.OrderId, cvref.OrderNo, cvref.OrderItemId,
                                   "Get Candidate's acceptance of the selection term " + "Application No. " + cvref.ApplicationNo + ", " +
                                   cvref.CandidateName + " selected for " + cvref.CustomerName + " on " + dateTimeNow.Date,
                                        dateTimeNow.AddDays(2), "Open", cvref.CandidateId, 0);
                              _unitOfWork.Repository<ApplicationTask>().Add(HRExecTask);
                              recAffected++;
                         }
                    
                         //A5 - Update CVRef record with deployment stage information
                         cvref.cvref.DeployStageId = (int)EnumDeployStatus.Selected;
                         cvref.cvref.DeployStageDate = dateTimeNow;
                    } else {
                         rejectedMsgsDto.Add(selectedMsgDto);
                    }
                    
                    //this flow is for all selection decisions, as against the above loop only for selected decision
                    //B1 - for all flows - selected and rejected, update CVRef with refStatus and RefStatusDate
                    cvref.cvref.RefStatus = s.SelectionStatusId;
                    cvref.cvref.RefStatusDate=dateTimeNow;
                    _unitOfWork.Repository<CVRef>().Update(cvref.cvref);
                    recAffected++;
                    
                    //b2 - update doc controller task for selection , whether seleced or rejected, teh task is over
                    var docTask = await _context.Tasks.Where(x => x.CandidateId == cvref.CandidateId &&
                         x.OrderItemId == cvref.OrderItemId && x.TaskTypeId == (int)EnumTaskType.SelectionFollowupWithClient)
                         .FirstOrDefaultAsync();
                    if (docTask != null)
                    {
                         docTask.TaskStatus = "Completed";
                         docTask.CompletedOn = dateTimeNow;
                         _unitOfWork.Repository<ApplicationTask>().Update(docTask);
                    }
               }

               var msg = new EmailMessage();
               var msgs = new List<EmailMessage>();
               var messages = new List<EmailMessage>();
               //C1 - compose message fo the selections

               if( selParams.SelectionEmailToCandidates && selectedMsgsDto.Count > 0)
               {
                    messages = await _composeMsgForAdmin.AdviseSelectionStatusToCandidateByEmail(selectedMsgsDto, loggedInUserName, dateTimeNow, m_SenderEmailAddresss, loggedInEmployeeId);
               }
               
               
               if (selParams.RejectionEmaiLToCandidates && rejectedMsgsDto.Count > 0)
               {
                    msgs = _composeMsgForAdmin.AdviseRejectionStatusToCandidateByEmail(rejectedMsgsDto, loggedInUserName, dateTimeNow, m_SenderEmailAddresss, loggedInEmployeeId);
                    if (msg != null && msgs.Count > 0 )
                    { foreach (var m in msgs) { messages.Add(m); } }
               }

               var empdtos = new List<EmploymentDto>();
               
               if(employments !=null && employments.Count > 0) {
                    empdtos = (List<EmploymentDto>)_mapper.Map<ICollection<Employment>, ICollection<EmploymentDto>>(employments);
               }

               if(messages.Count > 0 ) {
                    foreach(var m in messages) {
                         _unitOfWork.Repository<EmailMessage>().Add(m);
                    }
               }

               await _unitOfWork.Complete();
               //recAffected=await _context.SaveChangesAsync();
               var cvrefidsAffected = await _context.CVRefs.Where(x => cvrefids.Contains(x.Id) && x.RefStatusDate==dateTimeNow).Select(x => x.Id).ToListAsync();
               return new SelectionMsgsAndEmploymentsDto{EmailMessages = msgs, EmploymentDtos=empdtos, CvRefIdsAffected=cvrefidsAffected};

               
          }

          private async Task<string> getSalaryCurrency(int orderItemId)
          {
              var remuneration = await _context.Remunerations.Where(x => x.OrderItemId == orderItemId).Select(x => x.SalaryCurrency).FirstOrDefaultAsync();
              if (string.IsNullOrEmpty(remuneration)) return "Undefined";
              return remuneration;

          }
          public async Task<ICollection<SelectionStatus>> GetSelectionStatus()
          {
               return await _context.SelectionStatuses.OrderBy(x => x.Status).ToListAsync();
          }

          public async Task<Pagination<SelectionsPendingDto>> GetPendingSelections(CVRefSpecParams specParams)
          {
               var qry2 = (from r in _context.CVRefs
                    where r.RefStatus == (int)EnumCVRefStatus.Referred
                    join i in _context.OrderItems on r.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    //join c in _context.Customers on o.CustomerId equals c.Id
                    //join p in _context.Categories on i.CategoryId equals p.Id
                    join cand in _context.Candidates on r.CandidateId equals cand.Id
                    orderby r.ReferredOn
                    select new SelectionsPendingDto
                    {
                         CVRefId = r.Id,
                         OrderItemId = r.OrderItemId,
                         OrderNo = o.OrderNo,
                         CandidateName = cand.FullName,
                         ApplicationNo = cand.ApplicationNo,
                         CategoryRefAndName =o.OrderNo + "-" + i.SrNo+"-"+ i.Category.Name,
                         CustomerName = o.Customer.CustomerName,
                         ReferredOn = r.ReferredOn.Date,
                         RefStatus = r.RefStatus
                    })
                    .AsQueryable();
               var totalcount = await qry2.CountAsync();
               var data = await qry2.Skip((specParams.PageIndex -1)*specParams.PageSize).Take(specParams.PageSize)
                    .ToListAsync();

               /*
               specParams.CVRefStatus=(int)EnumCVRefStatus.Referred;

               var specs = new CVRefSpecs(specParams);
               var countSpec = new CVRefForCountSpecs(specParams);
               var totalItems = await _unitOfWork.Repository<CVRef>().CountAsync(countSpec);
               var cvs = await _unitOfWork.Repository<CVRef>().ListAsync(specs);

               //*PROJECTION HERE ?? **todo**
               */
               var dtos = new List<SelectionsPendingDto>();
               

               if(totalcount > 0) {

                    var refdetails = await (from i in _context.OrderItems where data.Select(x => x.OrderItemId).ToList().Contains(i.Id)
                         join o in _context.Orders on i.OrderId equals o.Id 
                         join c in _context.Customers on o.CustomerId equals c.Id 
                         join cat in _context.Categories on i.CategoryId equals cat.Id 
                         select new {CustomerName = c.CustomerName, OrderNo=o.OrderNo, orderitemid=i.Id,
                              CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + cat.Name}
                    ).ToListAsync();
                    
                    foreach(var dto in data) {
                         if (string.IsNullOrEmpty(dto.CustomerName)) dto.CustomerName= 
                              refdetails.Find(x => x.orderitemid == dto.OrderItemId).CustomerName;
                         if (string.IsNullOrEmpty(dto.CategoryRefAndName)) dto.CategoryRefAndName= 
                              refdetails.Find(x => x.orderitemid == dto.OrderItemId).CategoryRef;

                         dtos.Add(new SelectionsPendingDto {
                              CVRefId = dto.CVRefId,
                              OrderItemId = dto.OrderItemId,
                              OrderNo = dto.OrderNo,
                              CandidateName = dto.CandidateName,
                              ApplicationNo = dto.ApplicationNo,
                              CategoryRefAndName = dto.CategoryRefAndName,
                              CustomerName = dto.CustomerName,
                              ReferredOn = dto.ReferredOn.Date,
                              RefStatus = dto.RefStatus
                         });
                    }
               }
               
               return new Pagination<SelectionsPendingDto>(specParams.PageIndex, specParams.PageSize, totalcount, dtos);

          }
     
          public async Task<ICollection<EmailMessage>> ComposeSelectionEmailMessagesFromCVRefIds(ICollection<int> cvrefids, int loggedinEmpId, string loggedInUserName, DateTime datetimenow){

               var data = await GetSelectionDataMessageFromCVRefId(cvrefids);

               if (data==null || data.Count ==0) return null;

               return await _composeMsgForAdmin.AdviseSelectionStatusToCandidateByEmail(data, loggedInUserName, datetimenow, m_SenderEmailAddresss, loggedinEmpId);

          }
   
   
          public async Task<ICollection<EmailMessage>> ComposeRejEmailMessagesFromCVRefIds(ICollection<int> cvrefids, int loggedinEmpId, string loggedInUserName, DateTime datetimenow){

               var data = await GetSelectionDataMessageFromCVRefId(cvrefids);

               if (data==null || data.Count ==0) return null;

               return _composeMsgForAdmin.AdviseRejectionStatusToCandidateByEmail(data, loggedInUserName, datetimenow, m_SenderEmailAddresss, loggedinEmpId);

          }

          private async Task<ICollection<SelectionMessageDto>> GetSelectionDataMessageFromCVRefId(ICollection<int> cvrefids)
          {
               var details = (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id) && cvref.RefStatus==(int)EnumCVRefStatus.Selected
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                    //join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    //join c in _context.Customers on o.CustomerId equals c.Id
                    join empExec in _context.Employees on item.HrExecId equals empExec.Id
                    join empSup in _context.Employees on item.HrSupId equals empSup.Id  
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                    select new {
                         CVRefId=cvref.Id, CandidateName = cv.FullName, CandidateTitle="Mr.",     //**TODO*8 Add CandidateTitle to candidate
                         CandidateEmail = cv.Email, CandidateId=cv.Id, CandidateGender=cv.Gender, CandidateKnownAs=cv.KnownAs,
                         CategoryRef=o.OrderNo + "-" + item.SrNo + item.Category.Name,
                         CustomerName = o.Customer.CustomerName,  CustomerCity=o.Customer.City,
                         ApplicationNo = cv.ApplicationNo, Charges=item.Charges,
                         OrderItemId = cvref.OrderItemId, 
                         CategoryId = item.CategoryId, CategoryName = item.Category.Name,
                         OrderId=o.Id, OrderNo = o.OrderNo, 
                         SelectionStatusId=cvref.RefStatus,
                         HRExecId=empExec.Id, HRExecName=empExec.KnownAs, HRExecEmail=empExec.Email, HRExecGender=empExec.Gender,
                         HRSupId=empSup.Id, HRSupName=empSup.KnownAs, HRSupEmai=empSup.Email, HRSupGender=empSup.Gender
                    })
                    .ToList();
               
               if (details==null || details.Count ==0) return null;

               var selectedMsgsDto = new List<SelectionMessageDto>();
               
               foreach (var s in details)
               {
                    var cvref = details.Where(x => x.CVRefId== s.CVRefId)
                         .Select(x => new {x.CVRefId, x.OrderItemId,
                              x.OrderId, x.OrderNo, x.CustomerName,
                              x.CategoryId, x.CategoryName,
                              x.ApplicationNo, x.CandidateName, x.CandidateId, x.CandidateGender, x.CandidateEmail, 
                                   x.CandidateKnownAs, x.CandidateTitle, x.CustomerCity,
                              x.HRExecId, x.HRExecGender, x.HRExecName, x.HRExecEmail,
                              x.HRSupId, x.HRSupName, x.HRSupEmai, x.HRSupGender, x.SelectionStatusId
                         }).FirstOrDefault();

                    if (cvref.SelectionStatusId == (int)EnumCVRefStatus.Selected)
                    {
                         //A1 - create employment record
                         var salCurrency = await getSalaryCurrency(cvref.OrderItemId);
                         
                         var selectedMsgDto = new SelectionMessageDto(
                              cvref.CustomerName, cvref.CustomerCity, cvref.OrderNo, cvref.CategoryName, null, null,
                              cvref.ApplicationNo, cvref.CandidateId, cvref.CandidateTitle, cvref.CandidateName,
                              cvref.CandidateGender, cvref.CandidateEmail, cvref.CandidateKnownAs, cvref.HRExecId,
                              cvref.HRExecName, cvref.HRExecEmail, cvref.HRSupId, cvref.HRSupName, cvref.HRSupEmai
                         );
                         selectedMsgsDto.Add(selectedMsgDto);
                    } 
                    
               }

               return selectedMsgsDto;

          }
     }
}         