using AutoMapper;
using core.Dtos;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Extensions;
using core.Interfaces;
using core.Params;
using infra.Data;
using Microsoft.EntityFrameworkCore;


// TODO - constants for deploymentStageIds
namespace infra.Services
{
     public class DeployService : IDeployService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly IMapper _mapper;
          private readonly ICommonServices _commonServices;
          public DeployService(IUnitOfWork unitOfWork, ATSContext context, IMapper mapper, ICommonServices commonServices)
          {
               _commonServices = commonServices;
               _mapper = mapper;
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<Pagination<CVRefAndDeployDto>> GetPendingDeployments(DeployParams deployParams)
          {
               var qry = (from r in _context.CVRefs where r.RefStatus == (int)EnumCVRefStatus.Selected
                    join i in _context.OrderItems on  r.OrderItemId equals i.Id
                    join st in _context.DeployStages on r.DeployStageId equals st.Id
                    join c in _context.Candidates on r.CandidateId equals c.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    orderby r.OrderItemId, r.Id 
                    
                    select new CVRefAndDeployDto{
                         Checked=false, 
                         CVRefId = r.Id, 
                         CustomerName = cust.KnownAs, 
                         OrderId = i.OrderId, 
                         OrderNo = o.OrderNo, 
                         OrderDate = o.OrderDate,
                         OrderItemId = r.OrderItemId,
                         CategoryRef = o.OrderNo + "-" + i.SrNo,
                         CategoryName = cat.Name, 
                         CustomerId = o.CustomerId,
                         CandidateId = c.Id, 
                         ApplicationNo=c.ApplicationNo,
                         CandidateName = c.KnownAs, 
                         ReferredOn = r.ReferredOn, 
                         SelectedOn = r.RefStatusDate,
                         RefStatus = r.RefStatus,
                         DeployStageId  = (int)r.DeployStageId,
                         DeployStageName = st.Status,
                         TransactionDate = (DateTime)r.DeployStageDate,
                         NextStageId = st.NextDeployStageSequence,
                         NextStageDate = DateTime.Now
                    }
               ).AsQueryable();
               

                    if(deployParams.OrderNo > 0) qry = qry.Where(x => x.OrderNo == deployParams.OrderNo);
                    if(deployParams.ApplicationNo > 0) qry = qry.Where(x => x.ApplicationNo == deployParams.ApplicationNo);
                    if(!string.IsNullOrEmpty(deployParams.CandidateName)) qry = qry.Where(x => deployParams.CandidateName.ToLower().Contains(x.CandidateName.ToLower()));
                    if(!string.IsNullOrEmpty(deployParams.CategoryName)) qry = qry.Where(x => deployParams.CategoryName.ToLower().Contains(x.CategoryName.ToLower()));
                    if(!string.IsNullOrEmpty(deployParams.DeployStageName)) qry = qry.Where(x => deployParams.DeployStageName.ToLower().Contains(x.DeployStageName.ToLower()));
                    if(deployParams.TransactionDate.Year > 2000) qry = qry.Where(x => x.TransactionDate.Date == deployParams.TransactionDate.Date);
                    if(deployParams.SelectedOn.Year > 2000) qry = qry.Where(x => x.SelectedOn.Date == deployParams.SelectedOn.Date);

               
               var count = await qry.CountAsync();
               if (count == 0) return null;

               var data = await qry.Skip((deployParams.PageIndex-1)*deployParams.PageSize).Take(deployParams.PageSize).ToListAsync();
               /* var deployHeaders = new List<CVRefAndDeployDto>();
               var candidates = new List<CandidateSelected>();
               int lastOrderItemId=0;
               var Sels = new List<CandidateSelected>();
               var deployHeader = new CVRefAndDeployDto();

               foreach(var d in data) {
                    if (d.OrderItemId != lastOrderItemId) {
                         deployHeader = new CVRefAndDeployDto{
                              OrderId = d.OrderId, OrderNo = d.OrderNo, CompanyName = d.CustomerName,
                              OrderItemId = d.OrderItemId, CategoryRef = d.CategoryRef, CategoryName = d.CategoryName};
                         deployHeader.CompanyName=d.CustomerName;
                         lastOrderItemId=d.OrderItemId;
                         deployHeader.SelectedCandidates = new List<CandidateSelected>();
                         lastOrderItemId = d.OrderItemId;
                    } 
                    var candidate = new CandidateSelected{
                         Checked=false, CVRefId = d.CVRefId, CandidateId = d.CandidateId, ApplicationNo=d.ApplicationNo,
                         CandidateName = d.CandidateName, ReferredOn = d.ReferredOn, SelectedOn = d.SelectedOn,
                         SelectionId = 0, RefStatus = (EnumCVRefStatus)d.RefStatus,
                         Deploys = _mapper.Map<ICollection<Deploy>, ICollection<DeployDto>>(d.Deploys)
                    };
                    deployHeader.SelectedCandidates.Add(candidate);
                    deployHeaders.Add(deployHeader);
                    //candidates.Add(d.Candidates);
                    //deployHeader.SelectedCandidates=candidates;
               }
               */

               return new Pagination<CVRefAndDeployDto>(deployParams.PageIndex, deployParams.PageSize, count, data);
          }

          
          public async Task<int> CountOfPendingDeployments()
          {
               return await _context.CVRefs.Where(x => x.DeployStageId < (int)EnumDeployStatus.Concluded).CountAsync();
          }
		
          public async Task<DeploymentDtoWithErrorDto> AddDeploymentTransactions(ICollection<Deploy> deployPosts, int loggedInEmployeeId)
          {
               // A - if transDate missing, make it current date
               // B - Create a model based on input parameters,
               // C - verify stageId sequence, and update NextSTageId and NextSTageEstiamted date values
               // D - update DB
               // E - update CVRef.DeployStage values
               // F - Issue tasks for next process

               var depTOReturn = new List<DeploymentObjDto>();

               DateTime dt;
               var ErrorStringList= new List<string>();
               var succeeded=false;
               string err="";
               var deploys = new List<Deploy>();
               foreach(var post in deployPosts)
               {
                    dt = post.TransactionDate;

                    // B -
                         var nextDeployStage = await GetNextValidDeployStage(post);
                         
                         var ecnr = await _context.CVRefs.Where(x => x.Id==post.CVRefId).Select(x => x.Ecnr).FirstOrDefaultAsync();

                         if(ecnr=="ecnr" && post.StageId==EnumDeployStatus.TravelTicketBooked && 
                              (EnumDeployStatus)nextDeployStage.Id==EnumDeployStatus.EmigDocsLodgedOnLine) {

                         } else if((int)post.StageId != nextDeployStage.Id) {
                              ErrorStringList.Add("Out of sequence Error - Current Status " + post.StageId.GetEnumDisplayName() + 
                                   " should be followed by " + ((EnumDeployStatus)nextDeployStage.Id).GetEnumDisplayName());
                              continue;
                         }

                         var newDeploy= new Deploy(post.CVRefId, dt, post.StageId, (EnumDeployStatus) nextDeployStage.Id,
                              dt.AddDays(
                                   nextDeployStage.EstimatedDaysToCompleteThisStage==0 
                                   ? 2 
                                   : nextDeployStage.EstimatedDaysToCompleteThisStage)
                         );

                    // D - update DB
                         _unitOfWork.Repository<Deploy>().Add(newDeploy);
                         deploys.Add(newDeploy);

                    // E - update CVRef.Deploy fields
                         var cvref = await _context.CVRefs.FindAsync(post.CVRefId);
                         cvref.DeployStageId = (int)newDeploy.StageId;
                         cvref.DeployStageDate = newDeploy.TransactionDate;
                         _unitOfWork.Repository<CVRef>().Update(cvref);

                    // F - issue tasks
                         EnumTaskType thisTaskType=EnumTaskType.None;
                         int AssignedToId=0;
                         var commondata = await _commonServices.CommonDataFromCVRefId(post.CVRefId);
                         switch(newDeploy.NextStageId)
                         {
                              case EnumDeployStatus.Selected:
                                   thisTaskType = EnumTaskType.OfferLetterAcceptance;
                                   AssignedToId = commondata.HRExecId;
                                   break;

                              case EnumDeployStatus.OfferLetterAccepted:
                                   thisTaskType = EnumTaskType.MedicalTestMobiization;
                                   AssignedToId = commondata.MedicalProcessInchargeEmpId;
                                   break;

                              case EnumDeployStatus.ReferredForMedical:
                                   thisTaskType = EnumTaskType.MedicallyFit;
                                   AssignedToId = commondata.VisaProcessInchargeEmpId;
                                   break;
                              case EnumDeployStatus.VisaDocsPrepared:
                                   thisTaskType = EnumTaskType.VisaDocSubmission;
                                   AssignedToId = commondata.VisaProcessInchargeEmpId;
                                   break;
                              case EnumDeployStatus.VisaReceived:
                                   thisTaskType = EnumTaskType.VisaReceived;
                                   AssignedToId = commondata.TravelProcessInchargeId;
                                   break;
                              case EnumDeployStatus.EmigDocsLodgedOnLine:
                              case EnumDeployStatus.EmigrationGranted:
                                   thisTaskType = EnumTaskType.EmigDocmtsLodged;
                                   AssignedToId = commondata.TravelProcessInchargeId;
                                   break;
                              default:
                                   break;
                         }
                    
                         if(AssignedToId !=0)
                         {
                              var task = new ApplicationTask((int)thisTaskType, dt, loggedInEmployeeId, AssignedToId,
                                   commondata.OrderId, commondata.OrderNo, commondata.OrderItemId, "Task for you to organize " +
                                   ProcessName(post.NextStageId) + " for " + commondata.CandidateDesc, post.NextEstimatedStageDate,
                                   "Open", commondata.CandidateId, cvref.CVReviewId);
                              _unitOfWork.Repository<ApplicationTask>().Add(task);
                         }
               }
               // A - 
               succeeded=await _unitOfWork.Complete()>0;

               //if deploys.Add has Id value defined, then it is written to the DB
               foreach(var dep in deploys) {
                    if(dep.Id > 0) {
                         var dtoObj = new DeploymentObjDto();
                         dtoObj.CVRefId=dep.CVRefId;
                         dtoObj.Id=dep.Id;
                         dtoObj.NextEstimatedStageDate=dep.NextEstimatedStageDate;
                         dtoObj.NextStageId = (int)dep.NextStageId;
                         dtoObj.NextStageName= dep.NextStageId.GetEnumDisplayName();
                         dtoObj.StageName=dep.StageId.GetEnumDisplayName();
                         dtoObj.TransactionDate=dep.TransactionDate;
                         
                         depTOReturn.Add(dtoObj);
                    }
               }
               
               foreach(var st in ErrorStringList) {
                    if(!string.IsNullOrEmpty(err)) err=err + Environment.NewLine;
                    err +=st;
               }
          
               if (deploys.Count >0 && !string.IsNullOrEmpty(err)) err=deploys.Count + " deployment records registered, but others had following error(s):" + Environment.NewLine + err;
               
               var dtoToReturn = new DeploymentDtoWithErrorDto();

               dtoToReturn.DeploymentObjDtos= _mapper.Map<ICollection<Deploy>, ICollection<DeploymentObjDto>>(deploys);
               dtoToReturn.ErrorStrings=ErrorStringList;
               return dtoToReturn;
          }
         
          public async Task<bool> DeleteDeploymentTransactions(int deployid)
          {
               var deployToDelete = await _context.Deploys.FindAsync(deployid);
               if(deployToDelete==null) return false;
               
               var lst = new List<int>();
               lst.Add(deployToDelete.CVRefId);

               _unitOfWork.Repository<Deploy>().Delete(deployToDelete);

               await _unitOfWork.Complete();

               return await UpdateCVRefWithDeployLastRecords(lst);
          }

         
          public async Task<bool> EditDeploymentTransactions(ICollection<Deploy> deploys)
          {
               
               var deploysToUpdate=new List<Deploy>();
               bool updated=false;
               
               foreach(var deploy in deploys) {
                    var existingDeploy = await _context.Deploys.Where(x => x.Id == deploy.Id).AsNoTracking().FirstOrDefaultAsync();
                    if(existingDeploy ==null) continue;
                    
                    _context.Entry(existingDeploy).CurrentValues.SetValues(deploy);

                    _context.Entry(existingDeploy).State=EntityState.Modified;
                    
                    updated=true;
               }
               
               if(updated) {
                    await _context.SaveChangesAsync();

                    await UpdateCVRefWithDeployLastRecords(deploysToUpdate.Select(x => x.CVRefId).ToList());
                    
                    return true;
               }
               return false;
          }


          public async Task<CVRef> GetDeploymentsByCandidateAndOrderItem(int candidateId, int orderItemId)
          {
               return await _context.CVRefs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
               .Include(x => x.Deploys.OrderByDescending(x => x.TransactionDate)).FirstOrDefaultAsync();
          }

          public async Task<CVRef> GetDeploymentsById(int cvrefid)
          {
               return await _context.CVRefs.Where(x => x.Id == cvrefid)
               .Include(x => x.Deploys.OrderByDescending(x => x.TransactionDate)).FirstOrDefaultAsync();
          }

          public async Task<ICollection<DeploymentObjDto>> GetDeploymentsObject(int cvrefid)
          {
                var qry = await (from d in _context.Deploys where d.CVRefId==cvrefid 
                    join s in _context.DeployStages on (int)d.StageId equals s.Id 
                    join sn in _context.DeployStages on (int)d.NextStageId equals sn.Id
                    orderby d.TransactionDate descending
                    select new DeploymentObjDto {
                         Id = d.Id,
                         CVRefId = d.CVRefId, 
                         StageId = s.Id,
                         NextStageId=sn.Id,
                         TransactionDate = d.TransactionDate,
                         StageName=s.Status,
                         NextStageName=sn.Status,
                         NextEstimatedStageDate=d.NextEstimatedStageDate
                    }
               ).ToListAsync();

               return qry;
          }
          public async Task<CVReferredDto> GetDeploymentDto(int cvrefid)
          {
                var qry = (from r in _context.CVRefs where r.Id == cvrefid
                    join c in _context.Candidates on r.CandidateId equals c.Id
                    join i in _context.OrderItems on r.OrderItemId equals i.Id
                    join o in _context.Orders on i.OrderId equals o.Id
                    select new CVReferredDto {
                         CvRefId = cvrefid, CustomerName = o.Customer.CustomerName,
                         OrderId = o.Id, OrderDate = o.OrderDate, OrderItemId = i.Id,
                         CategoryName = i.Category.Name, CategoryRef = o.OrderNo + "-" + i.SrNo,
                         CustomerId = o.CustomerId, CandidateId = r.CandidateId, 
                         ApplicationNo = r.Candidate.ApplicationNo, CandidateName = r.Candidate.FullName,
                         ReferredOn = r.ReferredOn, SelectedOn = r.RefStatusDate,
                    })
                    .AsQueryable();
               var cvref = await qry.FirstOrDefaultAsync();
               var dep = await _context.Deploys.Where(x => x.CVRefId==cvrefid)
                    .OrderByDescending(x => x.TransactionDate).ToListAsync();

               var statuses = await _context.DeployStages.OrderBy(x => x.Sequence).Select(x => new {StageId = x.Id, StatusName = x.Status}).ToListAsync();

               var dtos = new List<DeployRefDto>();
               foreach(var d in dep)
               {
                    dtos.Add(new DeployRefDto {CvRefId = d.CVRefId,TransactionDate = d.TransactionDate, 
                         DeploymentStatusname = statuses.Find(x => x.StageId==(int)d.StageId).StatusName});
               }
               
               var dto = new CVReferredDto();
               dto = cvref;
               dto.Deployments = dtos;
               return dto;
               
               /*
               var q = await _context.CVRefs.Where(x => x.Id == cvrefid)
                    .Include(x => x.OrderItem).ThenInclude(y => y.Category)
                    .Include(z => z.Candidate)
                    .FirstOrDefaultAsync();
                    
               return q;
               */
               //return await qry.FirstOrDefaultAsync();
                    
          }
          
          
          //verify sequence of deployment stage id based upon last status
          //if ok, set next stage and date of next stage
          public async Task<DeployStage> GetNextValidDeployStage(Deploy model)
          {

               var validStage = await _context.DeployStages.Where(x => x.Id==(int)model.StageId).FirstOrDefaultAsync();

               return validStage;
          }

          
          private async Task<string> ProcessName(EnumDeployStatus ProcessId)
          {
               return await _context.DeployStages.Where(x => (EnumDeployStatus)x.Id == ProcessId).Select(x => x.Status).FirstOrDefaultAsync();
          }

          private async Task<bool> UpdateCVRefWithDeployLastRecords(ICollection<int> cvrefids)
          {
               foreach(var cvrefid in cvrefids) {
                    var deployLastRecord = await _context.Deploys.Where(x => x.CVRefId == cvrefid)
                    .OrderByDescending(x => x.TransactionDate).Take(1).FirstOrDefaultAsync();

                    var cvrefToUpdate = await _context.CVRefs.FindAsync(cvrefid);
                    cvrefToUpdate.DeployStageId = (int)deployLastRecord.StageId;
                    cvrefToUpdate.DeployStageDate = deployLastRecord.TransactionDate;

                    _unitOfWork.Repository<CVRef>().Update(cvrefToUpdate);
               }
               
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<DeployStatusDto>> GetDeployStatuses()
          {
               var lst = await _context.DeployStages.OrderBy(x => x.Sequence).Select(x => new DeployStatusDto{StageId=x.Id, StatusName=x.Status}).ToListAsync();
               return lst;
          }


	}
}