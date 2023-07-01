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

          public async Task<Pagination<DeploymentPendingDto>> GetPendingDeployments(DeployParams deployParams)
          {
               var qry = (from r in _context.CVRefs where r.RefStatus == (int)EnumCVRefStatus.Selected
                    join emp in _context.Employments on r.Id equals emp.CVRefId where emp.Approved
                    //join i in _context.OrderItems on  r.OrderItemId equals i.Id
                    //join st in _context.DeployStages on r.Sequence equals st.Id
                    join c in _context.Candidates on r.CandidateId equals c.Id
                    //join o in _context.Orders on i.OrderId equals o.Id
                    //join cust in _context.Customers on o.CustomerId equals cust.Id
                    //join cat in _context.Categories on i.CategoryId equals cat.Id
                    orderby r.OrderItemId, r.Id 
                    
                    select new DeploymentPendingDto {
                         CVRefId = r.Id, 
                         CustomerName = r.CustomerName, 
                         OrderNo = r.OrderNo, 
                         //OrderDate = r.OrderDate,
                         CategoryName = r.CategoryName, 
                         ApplicationNo=c.ApplicationNo,
                         CandidateName = c.KnownAs, 
                         ReferredOn = r.ReferredOn, 
                         DeploySequence  = (EnumDeployStatus)r.Sequence,
                         DeployStageDate = (DateTime)r.DeployStageDate,
                         NextSequence = (EnumDeployStatus)r.NextSequence,
                         NextStageDate = DateTime.Now
                    }
               ).AsQueryable();

               if(deployParams.OrderNo > 0) qry = qry.Where(x => x.OrderNo == deployParams.OrderNo);
               if(deployParams.ApplicationNo > 0) qry = qry.Where(x => x.ApplicationNo == deployParams.ApplicationNo);
               if(!string.IsNullOrEmpty(deployParams.CandidateName)) qry = qry.Where(x => deployParams.CandidateName.ToLower().Contains(x.CandidateName.ToLower()));
               if(!string.IsNullOrEmpty(deployParams.CategoryName)) qry = qry.Where(x => deployParams.CategoryName.ToLower().Contains(x.CategoryName.ToLower()));
               if(deployParams.TransactionDate.Year > 2000) qry = qry.Where(x => x.DeployStageDate.Date == deployParams.TransactionDate.Date);

               var count = await qry.CountAsync();
               if (count == 0) return null;

               var data = await qry.Skip((deployParams.PageIndex-1)*deployParams.PageSize).Take(deployParams.PageSize).ToListAsync();

               return new Pagination<DeploymentPendingDto>(deployParams.PageIndex, deployParams.PageSize, count, data);
          }

          
          public async Task<int> CountOfPendingDeployments()
          {
               return await _context.CVRefs.Where(x => x.Sequence < (int)EnumDeployStatus.Concluded).CountAsync();
          }
		
          public async Task<DeploymentDtoWithErrorDto> AddDeploymentTransactions(ICollection<Deploy> deployPosts, int loggedInEmployeeId)
          {
               // A - if transDate missing, make it current date
               // B - Create a model based on input parameters,
               // C - verify Sequence sequence, and update NextSequence and NextSTageEstiamted date values
               // D - update DB
               // E - update CVRef.DeployStage values
               // F - Issue tasks for next process

               var depTOReturn = new List<DeploymentDto>();

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

                         if(ecnr=="ecnr" && post.Sequence==EnumDeployStatus.TravelTicketBooked && 
                              (EnumDeployStatus)nextDeployStage.Id==EnumDeployStatus.EmigDocsLodgedOnLine) {

                         } else if((int)post.Sequence != nextDeployStage.Id) {
                              ErrorStringList.Add("Out of sequence Error - Current Status " + post.Sequence.GetEnumDisplayName() + 
                                   " should be followed by " + ((EnumDeployStatus)nextDeployStage.Id).GetEnumDisplayName());
                              continue;
                         }

                         var newDeploy= new Deploy(post.CVRefId, dt, post.Sequence, (EnumDeployStatus) nextDeployStage.Id,
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
                         cvref.Sequence = (int)newDeploy.Sequence;
                         cvref.DeployStageDate = newDeploy.TransactionDate;
                         _unitOfWork.Repository<CVRef>().Update(cvref);

                    // F - issue tasks
                         EnumTaskType thisTaskType=EnumTaskType.None;
                         int AssignedToId=0;
                         var commondata = await _commonServices.CommonDataFromCVRefId(post.CVRefId);
                         switch(newDeploy.NextSequence)
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
                                   ProcessName(post.NextSequence) + " for " + commondata.CandidateDesc, post.NextStageDate,
                                   "Open", commondata.CandidateId, cvref.CVReviewId);
                              _unitOfWork.Repository<ApplicationTask>().Add(task);
                         }
               }
               // A - 
               succeeded=await _unitOfWork.Complete()>0;

               //if deploys.Add has Id value defined, then it is written to the DB
               foreach(var dep in deploys) {
                    if(dep.Id > 0) {
                         var dtoObj = new DeploymentDto();
                         dtoObj.CVRefId=dep.CVRefId;
                         dtoObj.Id=dep.Id;
                         dtoObj.NextStageDate=dep.NextStageDate;
                         dtoObj.NextSequence = (int)dep.NextSequence;
                         //dtoObj.NextStageName= dep.NextSequence.GetEnumDisplayName();
                         //dtoObj.StageName=dep.Sequence.GetEnumDisplayName();
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

               dtoToReturn.DeploymentObjDtos= _mapper.Map<ICollection<Deploy>, ICollection<DeploymentDto>>(deploys);
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

          public async Task<bool> EditDeploymentTransaction(DeploymentDto dto) {
               
               var deploy = new Deploy{Id=dto.Id, CVRefId = dto.CVRefId, Sequence= (EnumDeployStatus)dto.Sequence,
                    NextSequence=(EnumDeployStatus) dto.NextSequence,
                    TransactionDate=dto.TransactionDate, NextStageDate=dto.NextStageDate};

               if(deploy.Id==0) {
                    _context.Entry(deploy).State=EntityState.Added;
               } else {

                    var existingDeploy = await _context.Deploys.Where(x => x.Id == deploy.Id).AsNoTracking().FirstOrDefaultAsync();
                    if(existingDeploy==null) return false;
                    
                    _context.Entry(existingDeploy).CurrentValues.SetValues(deploy);

                    _context.Entry(existingDeploy).State=EntityState.Modified;
                    
               }
               
               //update CVRef.Sequence, NextSequence, LastDt
               var cvref = await _context.CVRefs.Where(x => x.Id==deploy.CVRefId).AsNoTracking().FirstOrDefaultAsync();
               if (cvref==null) return false;
               
               cvref.Sequence= (int)deploy.Sequence;
               cvref.NextSequence=(int)deploy.NextSequence;
               cvref.DeployStageDate=deploy.TransactionDate;
               
               _context.Entry(cvref).State=EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;
                    
          }

    
          public async Task<bool> EditDeploymentTransactions(CVReferredDto model)
          {
               var existingDeployments = await _context.Deploys.Where(x => x.CVRefId == model.CvRefId)
                    .AsNoTracking().ToListAsync();
               
               foreach (var item in model.Deployments)
               {

                    var existingItem = existingDeployments.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       // Update child
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(item);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else            //insert children as new record
                    {
                         var newItem = new Deploy(item.CVRefId, item.TransactionDate, (EnumDeployStatus)item.Sequence, 
                              (EnumDeployStatus)item.NextSequence, item.NextStageDate);
                         
                         existingDeployments.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }
               
               _context.Entry(existingDeployments).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;

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

          public async Task<ICollection<DeploymentDto>> GetDeployments(int cvrefid)
          {
               /* var temp= await (from d in _context.Deploys where d.CVRefId==cvrefid
                    select new {
                         Id = d.Id,
                         CVRefId = d.CVRefId, 

                         TransactionDate = d.TransactionDate,
                         NextStageName="", //sn.Status,
                         NextEStageDate=d.NextStageDate
                    }
                ).ToListAsync();
                */
                var qry = await (from d in _context.Deploys where d.CVRefId==cvrefid 
                    join s in _context.DeployStages on (int)d.Sequence equals s.Sequence
                    //join sn in _context.DeployStages on (int)d.NextSequence equals sn.Id
                    orderby d.TransactionDate descending
                    select new DeploymentDto {
                         Id = d.Id,
                         CVRefId = d.CVRefId, 
                         Sequence = s.Sequence,
                         NextSequence= s.NextSequence,
                         TransactionDate = d.TransactionDate,
                         //StageName=s.Status,
                         //NextStageName="", //sn.Status,
                         NextStageDate=d.NextStageDate
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

               var statuses = await _context.DeployStages.OrderBy(x => x.Sequence).Select(x => new {Sequence = x.Id, StatusName = x.Status}).ToListAsync();

               var dtos = new List<DeployDto>();
               foreach(var d in dep)
               {
                    dtos.Add(new DeployDto (d.Id, d.CVRefId, d.TransactionDate, Convert.ToInt32(d.Sequence),
                         Convert.ToInt32(d.NextSequence), d.NextStageDate));
               }
               
               var dto = new CVReferredDto();
               dto = cvref;
               dto.Deployments = dtos;
               return dto;
                    
          }
          
          
          //verify sequence of deployment stage id based upon last status
          //if ok, set next stage and date of next stage
          public async Task<DeployStage> GetNextValidDeployStage(Deploy model)
          {

               var validStage = await _context.DeployStages.Where(x => x.Id==(int)model.Sequence).FirstOrDefaultAsync();

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
                    cvrefToUpdate.Sequence = (int)deployLastRecord.Sequence;
                    cvrefToUpdate.DeployStageDate = deployLastRecord.TransactionDate;

                    _unitOfWork.Repository<CVRef>().Update(cvrefToUpdate);
               }
               
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<DeployStage>> GetDeployStatuses()
          {
               var lst = await _context.DeployStages.OrderBy(x => x.Sequence).ToListAsync();
               return lst;
          }


	}
}