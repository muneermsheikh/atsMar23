using AutoMapper;
using core.Dtos;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.Tasks;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace infra.Services
{
     public class ProspectiveCandidateService : IProspectiveCandidateService
     {
          private readonly ATSContext _context;
          private readonly UserManager<AppUser> _userManager;
          private readonly IUserService _userService;
          private readonly ITokenService _tokenService;
          private readonly ITaskService _taskService;
          private readonly int _ReceptionistEpmployeeId = 10;
          private readonly IComposeMessageForCandidates _composeMsg;
          private readonly IMapper _mapper;
          public ProspectiveCandidateService(ATSContext context, UserManager<AppUser> userManager, 
               IUserService userService, ITaskService taskService, ITokenService tokenService, 
               IComposeMessageForCandidates composeMsg, IMapper mapper)
          {
               _composeMsg = composeMsg;
               _mapper = mapper;
               _tokenService = tokenService;
               _userService = userService;
               _userManager = userManager;
               _context = context;
               _taskService = taskService;
          }

          public async Task<bool> ConvertProspectToCandidateFromProspectiveId(int id)
          {

               var profile = await(from p in _context.ProspectiveCandidates.Where(x => x.Id == id)
                    join i in _context.OrderItems on p.OrderItemId equals i.Id 
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new {p, i.CategoryId, cat.Name}
                    ).FirstOrDefaultAsync();

               DateTime dob = new DateTime();
               var age = profile.p.Age.Substring(0,2);

               if (!string.IsNullOrEmpty(age)) {
                    dob = DateTime.Today.AddYears(-Convert.ToInt32(age));
               }

               var profs = new List<UserProfession>();
               profs.Add(new UserProfession{CategoryId=profile.CategoryId});
               var phs = new List<UserPhone>();
               phs.Add(new UserPhone{MobileNo=profile.p.PhoneNo,IsMain=true});
               if(!string.IsNullOrEmpty(profile.p.AlternatePhoneNo)) phs.Add(new UserPhone{MobileNo=profile.p.AlternatePhoneNo});

               return false;
          }

          private async Task<UserProfession> GetCategoryIdAndNameFromCategoryRef(string categoryref)
          {
               if(string.IsNullOrEmpty(categoryref)) return null;
               
               int i = categoryref.IndexOf("-");
               if (i== -1) return null;
               var orderno = categoryref.Substring(0,i);
               var srno = categoryref.Substring(i+1);
               if (string.IsNullOrEmpty(orderno)) return null;
               if (string.IsNullOrEmpty(srno)) return null;
               int iorderno = Convert.ToInt32(orderno);
               int isrno = Convert.ToInt32(srno);

               var qry = await (from o in _context.Orders where o.OrderNo == iorderno 
                    join item in _context.OrderItems on o.Id equals item.OrderId 
                    join c in _context.Categories on item.CategoryId equals c.Id
                    select new {item.CategoryId, c.Name}).FirstOrDefaultAsync();
              
               if (qry == null) return null;

               return new UserProfession{CategoryId=qry.CategoryId, Profession=qry.Name };

          }

          private int GetAgencyIdFromAgencyName (string agencyname)
          {
               var id = _context.Customers
                    .Where(x => x.CustomerName.ToLower() == agencyname.ToLower())
                    .Select(x => x.Id)
                    .FirstOrDefault();
               return id;
          }
          
          public async Task<UserDto> ConvertProspectiveToCandidate(ProspectiveCandidateAddDto dto) 
          {
               //check unique values of PP and Aadhar
               var user = await _userManager.FindByEmailAsync(dto.Email);
               if(user == null) {            //possible if AppUser created, but failed to create Candidate object;
                              //**TODO** why note delete AppUser when Candidate failed to create??
               //Create AppUser before creating the candidate object, bvz canddidate contains AppuserId field
                    user = new AppUser
                    {
                         UserType = "Candidate",
                         DisplayName = dto.KnownAs,
                         KnownAs = dto.KnownAs,
                         Gender = dto.Gender,
                         PhoneNumber = dto.PhoneNo,
                         Email = dto.Email,
                         UserName = dto.Email
                    };
                    var result = await _userManager.CreateAsync(user, dto.Password);
                    if (!result.Succeeded) return null;
               }

               //create roles
               //var succeeded = await _roleManager.CreateAsync(new AppRole{Name="Candidate"});
               var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");
               //if (!roleResult.Succeeded) return null;
                    
               //var userAdded = await _userManager.FindByEmailAsync(registerDto.Email);
               //no need to retreive obj from DB - the object 'user' can be used for the same
               
               var cvDto = new RegisterDto{
                    UserType="Candidate", Gender="M", FirstName = dto.CandidateName, KnownAs = dto.KnownAs, 
                    DisplayName = dto.KnownAs, UserName = dto.Email,
                    Email = dto.Email,UserRole="Candidate", AppUserId = user.Id};


               if (!string.IsNullOrEmpty(dto.Age)) {
                    var age = dto.Age.Substring(0,2);
                    cvDto.DOB = DateTime.Today.AddYears(-Convert.ToInt32(age));
               }
               
               //EntityAddresses
               if (!string.IsNullOrEmpty(dto.CurrentLocation)) {
                    var adds = new List<EntityAddress>();
                    adds.Add(new EntityAddress{City=dto.CurrentLocation});
                    //cvDto.EntityAddresses = adds;
               }
               
               //ReferredBy
               int agencyid=0;
               switch (dto.Source.ToLower()) {
                    case "timesjob":
                    case "timesjobs":
                    case "timesjob.com":
                    case "timesjobs.com":
                         agencyid=9;
                         break;
                    case "naukri":
                    case "naukri.com":
                         agencyid=12;
                         break;
                    default:
                         break;
               }
               cvDto.ReferredBy=agencyid;

               
               // finally, create the object candidate
               var cand = await _userService.CreateCandidateObject(cvDto, dto.LoggedInUserId);

               if (cand == null) return null;
               
               _context.Entry(cand).State = EntityState.Added;

               //once succeeded, delete the record from prospective list.

               var prospect = await _context.ProspectiveCandidates.FindAsync(dto.ProspectiveId);
               if(prospect == null) return null;

               _context.Entry(prospect).State=EntityState.Deleted;

               var recordsAffected = await _context.SaveChangesAsync();         //should be 2 - one added, and one deleted

               //UserProfessions
               var prf = await GetCategoryIdAndNameFromCategoryRef(dto.CategoryRef);
               if (prf != null) {
                    var prof = new UserProfession{CategoryId=prf.CategoryId, CandidateId=cand.Id, Profession=prf.Profession};
                    _context.Entry(prof).State = EntityState.Added;
               }

               //UserPhones
               var ph = new UserPhone{MobileNo=dto.PhoneNo,IsMain=true, CandidateId=cand.Id};
               _context.Entry(ph).State=EntityState.Added;
               if(!string.IsNullOrEmpty(dto.AlternatePhoneNo)) {
                    ph =new UserPhone{MobileNo=dto.AlternatePhoneNo, CandidateId=cand.Id};
                    _context.Entry(ph).State=EntityState.Added;
               }

               await _context.SaveChangesAsync();

               //return UserDto object
               var userDtoToReturn = new core.Dtos.UserDto
               {
                    DisplayName = user.DisplayName,
                    Token = await _tokenService.CreateToken(user),
                    Email = user.Email
               };

               return userDtoToReturn;
          }

          public async Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams sParams)
          {
               var qry = _context.ProspectiveCandidates
                    .GroupBy(g => new {g.CategoryRef, g.Source , g.Date, g.Status})
                    .Where(g => g.Count() > 0)
                    .Select(g => new{g.Key, count = g.Count()})
                    .AsQueryable();

               var objTotal = await qry.ToListAsync();
               
               var summary = new List<ProspectiveSummaryDto>();
               foreach(var q in objTotal) {
                    var summaryItem = summary.Find(x => x.CategoryRef==q.Key.CategoryRef && x.Source == q.Key.Source && x.Date == q.Key.Date);
                    if (summaryItem == null) {
                         var sNew = new ProspectiveSummaryDto();
                         sNew.CategoryRef=q.Key.CategoryRef;
                         sNew.Date=(DateTime)q.Key.Date;
                         sNew.Source= q.Key.Source;
                         sNew.Status=q.Key.Status;
                         if (q.Key.Status.ToLower()=="notinterested") {
                              sNew.NotInterested=q.count;
                         } else if (q.Key.Status.ToLower()=="notresponding") {
                              sNew.NotResponding=q.count;
                         } else if(q.Key.Status.ToLower()=="pending") {
                              sNew.Pending=q.count;
                         } else if (q.Key.Status.ToLower()=="concluded") {
                              sNew.Concluded=q.count;
                         } else if (q.Key.Status.ToLower()=="ppissues") {
                              sNew.PpIssues=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenumberwrong") {
                              sNew.PhoneNoWrong=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                              sNew.PhoneNotReachable=q.count;
                         } else if (q.Key.Status.ToLower()=="scnotacceptable") {
                              sNew.ScNotAcceptable=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                              sNew.PhoneNotReachable=q.count;
                         } else if (q.Key.Status.ToLower()=="salaryofferedislow") {
                              sNew.LowSalary=q.count;
                         } else {
                              sNew.Others=q.count;
                         }
                         summary.Add(sNew);
                    } else {
                         if (q.Key.Status.ToLower()=="notinterested") {
                              summaryItem.NotInterested+=q.count;
                         } else if (q.Key.Status.ToLower()=="notresponding") {
                              summaryItem.NotResponding+=q.count;
                         } else if(q.Key.Status.ToLower()=="pending") {
                              summaryItem.Pending+=q.count;
                         } else if (q.Key.Status.ToLower()=="concluded") {
                              summaryItem.Concluded+=q.count;
                         } else if (q.Key.Status.ToLower()=="ppissues") {
                              summaryItem.PpIssues+=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenumberwrong") {
                              summaryItem.PhoneNoWrong+=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                              summaryItem.PhoneNotReachable+=q.count;
                         } else if (q.Key.Status.ToLower()=="scnotacceptable") {
                              summaryItem.ScNotAcceptable+=q.count;
                         } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                              summaryItem.PhoneNotReachable+=q.count;
                         } else if (q.Key.Status.ToLower()=="salaryofferedislow") {
                              summaryItem.LowSalary+=q.count;
                         } else {
                              summaryItem.Others+=q.count;
                         }
                    }
               }
               
               //var oPaged = summary.Take(sParams.PageSize).Skip(sParams.PageIndex-1).ToList();

               foreach(var smr in summary)
               {
                    smr.Total = smr.AskedToReachHimLater+smr.Concluded+smr.LowSalary+smr.NotInterested+smr.NotResponding+smr.Others+
                         smr.Pending+smr.PhoneNotReachable+smr.PhoneNoWrong+smr.PhoneUnanswered+smr.PpIssues+smr.ScNotAcceptable;
               }
               //var pagination = new Pagination<ProspectiveSummaryDto>(sParams.PageIndex, sParams.PageSize, summary.Count, oPaged) ;
               return summary;
          }

          
          public async Task<Pagination<ProspectiveCandidateEditDto>> GetProspectiveCandidates(ProspectiveCandidateParams pParams)
          {
               var qry = _context.ProspectiveCandidates.AsQueryable();
               
               if(!string.IsNullOrEmpty(pParams.CategoryRef) ) qry = qry.Where(x => x.CategoryRef == pParams.CategoryRef);
               
               if(!string.IsNullOrEmpty(pParams.Search)) qry = qry.Where(x => x.CategoryRef == pParams.Search);
               
               if(!string.IsNullOrEmpty(pParams.Status)) {
                    if(pParams.Status=="Others") {
                         string[] strOthers = {"notinterested","notresponding","pending", "concluded", "ppissues",
                              "phonenumberwrong", "phonenotreachable", "scnotacceptable", "phonenotreachable","salaryofferedislow"};
                         qry = qry.Where(x => !strOthers.Contains(x.Status));
                    } else if(pParams.Status !="all") {
                         qry = qry.Where(x => x.Status==pParams.Status);
                    }
               }
               
               if(pParams.DateAdded.Year > 2000) qry = qry.Where(x => x.Date == pParams.DateAdded);
               
               if(pParams.Id.HasValue) qry = qry.Where(x => x.Id == pParams.Id);
               if(pParams.HeaderId.HasValue) qry = qry.Where(x => x.Id == pParams.HeaderId);
/*
               switch(pParams.Sort.ToLower()) {
                    case "date":
                         qry = qry.OrderBy(x => x.Date).ThenBy(x => x.Status);
                         break;
                    case "city":
                         qry = qry.OrderBy(x => x.City).ThenBy(x => x.Status);
                         break;
                    case "status":
                         qry = qry.OrderBy(x => x.Status).ThenBy(X => X.CategoryRef);
                         break;
                    case "name":
                         qry = qry.OrderBy(x => x.CandidateName).ThenBy(x => x.Status);
                         break;
                    case "categoryref":
                         qry = qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status);
                         break;
                    default:
                         qry = qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status);
                         break;
               }
*/
               var totalCount = await qry.CountAsync();
               var prospectives = await qry.Skip((pParams.PageIndex-1)*pParams.PageSize).Take(pParams.PageSize).ToListAsync();
               
               var data = _mapper.Map<IReadOnlyList<ProspectiveCandidateEditDto>>(prospectives);

               return new Pagination<ProspectiveCandidateEditDto>(pParams.PageIndex, pParams.PageSize, totalCount, data);
          }

          public async Task<Pagination<UserHistoryHeaderDto>> GetCallRecordHeaders(UserHistoryHeaderParams hParams)
          {
               var qry = (from h in _context.UserHistoryHeaders
                    select new UserHistoryHeaderDto  {
                         Id = h.Id, CategoryRefCode = h.CategoryRefCode, CategoryRefName = h.CategoryRefName,
                         AssignedByName = h.AssignedByName, AssignedToName = h.AssignedToName, AssignedOn = h.AssignedOn, 
                         CompleteBy = h.CompleteBy, Concluded = h.Concluded, Status = h.Status
                    }).AsQueryable();

               if(!string.IsNullOrEmpty(hParams.Status)) qry = qry.Where(X => X.Status == hParams.Status);
               //if(hParams.Concluded.HasValue ) qry = qry.Where(x => x.Concluded == hParams.Concluded);
               qry = qry.OrderBy(x => x.AssignedOn);

               var totalCount = await qry.CountAsync();
               var result = await qry.Skip((hParams.PageIndex-1)*hParams.PageSize).Take(hParams.PageSize).ToListAsync();

               return new Pagination<UserHistoryHeaderDto>(hParams.PageIndex, hParams.PageSize, totalCount, result);

          }        

          public async Task<string> EditProspectiveCandidates(ICollection<ProspectiveUpdateDto> models, LoggedInUserDto userdto)
          {
               int count=0;
               var tparams = new TaskParams();
               tparams.TaskOwnerId = userdto.LoggedInEmployeeId;
               tparams.TaskDescription = "status changed on " + DateTime.Today;
               tparams.AssignedToId = _ReceptionistEpmployeeId;
               tparams.PersonType = "Prospective";
               tparams.TaskStatus = "Not started";
               tparams.CompleteBy = DateTime.Today.AddDays(2);
               tparams.TaskTypeId = 18;      //prospective

               var tNow = DateTime.Now;

               var ids = models.Select(x => x.ProspectiveId).ToList();

               var prospectivesExisting = await _context.ProspectiveCandidates.Where(x => ids.Contains(x.Id)).ToListAsync();
               foreach(var model in models)
               {
                    if (!string.IsNullOrEmpty(model.NewStatus)) 
                    {
                         var prospectiveExisting = prospectivesExisting.Where(x => x.Id == model.ProspectiveId).FirstOrDefault();
                         if(prospectiveExisting==null) continue;
                         
                         if(model.Closed) model.NewStatus="Concluded";

                         //create a new record for applicationTask
                         tparams.ResumeId = prospectiveExisting.ResumeId;

                         var t = await _taskService.GetOrCreateTaskFromParams(tparams, userdto.LoggedInEmployeeId, userdto.LoggedIAppUsername);
                         if(t!=null) {
                         var titem = new TaskItem{
                              ApplicationTaskId = t.Id,
                              TaskTypeId = (int)tparams.TaskTypeId, TransactionDate= tNow, TaskStatus =  model.NewStatus, 
                              TaskItemDescription = string.IsNullOrEmpty(model.Remarks) ? "No description available" : model.Remarks , 
                               OrderItemId= prospectiveExisting.OrderItemId, UserId = userdto.LoggedInEmployeeId,
                              UserName= userdto.LoggedIAppUsername};
                              
                              _context.TaskItems.Add(titem);
                         }
                         //update ProspectiveCandidate Database
                         prospectiveExisting.Status = model.NewStatus;
                         prospectiveExisting.StatusByUserId= userdto.LoggedInEmployeeId;
                         prospectiveExisting.StatusDate= tNow;
                         _context.Entry(prospectiveExisting).State=EntityState.Modified;

                         count++;
                    }
               }

               if(count==0) return "";

               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    return ex.Message;
               }

               //post actions for records withs tatusDate=tNow to make sure only those records are considered which are saved to DB
               var prospectivesSaved = await _context.ProspectiveCandidates.Where(x => ids.Contains(x.Id) && x.StatusDate ==tNow ).ToListAsync();

               if(prospectivesSaved==null) return "failed to save any of the contact transactions";

               var mails = new List<EmailMessage>();
               var smsMessages = new List<SMSMessage>();

               foreach(var item in prospectivesSaved)
               {
                    var candidateDetail = new ComposeMessageDtoForProspects(item.Id, item.CandidateName, item.Email, item.PhoneNo, item.Source, item.City, "India");
                    switch(item.Status) {
                         case "Interested in Offer":
                         case "Interested":
                              smsMessages.Add(_composeMsg.ComposeSMSForConsentOfInterest(candidateDetail, userdto));
                              mails.Add(_composeMsg.ComposeMessagesForConsentOfInterest(candidateDetail, userdto));
                              break;
                         case "Phone Not Reachable":
                         case "Phone unanswered":
                         case "Phone out of network":
                              mails.Add(_composeMsg.ComposeMessagesForFailureToReach(candidateDetail, userdto));
                              smsMessages.Add(_composeMsg.ComposeSMSForFailureToReach(candidateDetail, userdto));
                              break;
                         case "Phone Number Wrong":
                              mails.Add(_composeMsg.ComposeMessagesForFailureToReach(candidateDetail, userdto));
                              item.PhoneNo = item.PhoneNo + "-*invalid";
                              _context.Entry(item).State = EntityState.Modified;
                              break;
                         case "Not Interested in the project":
                         case "Not interested due to unknown reasons":
                              mails.Add(_composeMsg.ComposeMessageForNoInterest(candidateDetail, userdto));
                              break;
                         case "Service Charges not acceptable":
                              mails.Add(_composeMsg.ComposeMessageForSCNotAcceptable(candidateDetail, userdto));
                              break;
                         case "PP not in his possession":
                              break;

                         case "Salary offered is low":
                              mails.Add(_composeMsg.ComposeMessageForDeclinedDueToowSalary(candidateDetail, userdto));
                              break;
                         case "Does not sound interested":
                              break;
                         case "Rude behavior":
                              break;
                         case "Not interested for overseas job":
                              mails.Add(_composeMsg.ComposeMessageForNotInterestedForOverseasJob(candidateDetail, userdto));
                              break;
                         case "Asked to reach him later":
                              mails.Add(_composeMsg.ComposeMessageForAskToReachLater(candidateDetail, userdto));
                              smsMessages.Add(_composeMsg.ComposeSMSForAskToReachLater(candidateDetail, userdto));
                              break;
                         /* - not relevant for prospective candidates
                         case "Offer Accepted":
                              break;
                         
                         case "Positive - In progress":
                              break;
                         */

                         default:
                              break;
                    }
               }

               if(mails.Count > 0 || smsMessages.Count > 0) {
                    foreach(var item in mails) {
                         _context.EmailMessages.Add(item);
                    }
                    foreach(var item in smsMessages) {
                         _context.SMSMessages.Add(item);
                    }
                    await _context.SaveChangesAsync();
               }
               
               return "";
          }

          

	}
}