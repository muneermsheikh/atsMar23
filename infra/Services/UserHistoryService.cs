using core.Entities.Admin;
using core.Entities.Users;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using core.Params;
using AutoMapper;
using core.Entities.HR;
using core.Entities.EmailandSMS;
using core.Entities.Identity;

namespace infra.Services
{
     public class UserHistoryService : IUserHistoryService
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICommonServices _commonServices;
          private readonly IMapper _mapper;
          public IUserService UserService { get; }
          private readonly IUserService _userService;
          private readonly IComposeMessageForCandidates _composeProspectiveMsg;
          private readonly IComposeMessagesForHR _composeHRMsg;
          private readonly ISMSService _smsService;
          public UserHistoryService(ATSContext context, IUnitOfWork unitOfWork, ICommonServices commonServices,  
               IUserService userService, IMapper mapper, IComposeMessageForCandidates composeProspectiveMsg, 
               IComposeMessagesForHR composeHRMsg, ISMSService smsService)
          {
               _userService = userService;
               UserService = userService;
               _mapper = mapper;
               _commonServices = commonServices;
               _unitOfWork = unitOfWork;
               _context = context;
               _composeProspectiveMsg = composeProspectiveMsg;
               _composeHRMsg = composeHRMsg;
               _smsService = smsService;
          }

          
           public async Task<UserHistory> AddUserContact(UserHistory userContact)
          {
               if(!string.IsNullOrEmpty(userContact.ResumeId)) {
                    var user = await _context.UserHistories.Where(X => X.ResumeId==userContact.ResumeId).FirstOrDefaultAsync();
                    if (user == null) return null;
                    userContact.Name = user.Name;
               } else if (!string.IsNullOrEmpty(userContact.EmailId) && !string.IsNullOrEmpty(userContact.MobileNo) ){
                    var user = await _context.UserHistories.Where(x => 
                         x.EmailId == userContact.EmailId && x.MobileNo==userContact.MobileNo && x.Name==userContact.Name)
                         .FirstOrDefaultAsync();
                    if (user != null) return null;
               }
               
               _unitOfWork.Repository<UserHistory>().Add(userContact);
               
               if (await _unitOfWork.Complete() > 0) return userContact;
               
               return null;
          }
          

          public async Task<bool> DeleteUserContact(UserHistory userContact)
          {
               var contact = await _context.UserHistories.FindAsync(userContact.Id);
               if (contact == null) throw new Exception("invalid object");

               _unitOfWork.Repository<UserHistory>().Delete(userContact);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> DeleteUserContactById (int userContactId)
          {
               var contact = await _context.UserHistories.FindAsync(userContactId);
               if (contact == null) throw new Exception("invalid object");

               _unitOfWork.Repository<UserHistory>().Delete(contact);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<ICollection<ContactResult>> GetContactResults()
          {
               var results = await _context.ContactResults.OrderBy(x => x.Name).ToListAsync();
               return results;
          }

        
          public async Task<UserHistory> GetHistoryWithItemsFromHistoryId(int historyId)
          {
               var history = await _context.UserHistories
                    .Where(x => x.Id == historyId)
                    .Include(x => x.UserHistoryItems)
                    .FirstOrDefaultAsync();
               return history;     //_mapper.Map<UserHistory, UserHistoryDto>(history);
          }
          
          private async Task<UserHistory> GetUserHistory(UserHistoryParams histParams)
          {
               var qry = _context.UserHistories.AsQueryable();
               if (histParams.ApplicationNo.HasValue) {
                    qry =  qry.Where(x =>x.ApplicationNo == histParams.ApplicationNo);
               } else {
                    if (!string.IsNullOrEmpty(histParams.MobileNo)) qry = qry.Where(x => x.MobileNo == histParams.MobileNo);
                    if (!string.IsNullOrEmpty(histParams.EmailId)) qry = qry.Where(x => x.EmailId.ToLower() == histParams.EmailId.ToLower());
                    //if(histParams.PersonId.HasValue) qry = qry.Where(x => x.PersonId==histParams.PersonId);
               }
               qry = qry.Include(x => x.UserHistoryItems);
               var q = await qry.FirstOrDefaultAsync();
               return q;
          }
          
          public async Task<Pagination<UserHistoryDto>> GetUserHistoryPaginated(UserHistoryParams pParams)
          {
               
               var qry = _context.UserHistories.AsQueryable();
               if(pParams.Concluded != null) qry = qry.Where(x=> x.Concluded == pParams.Concluded);
               if(pParams.UserHistoryHeaderId.HasValue) qry = qry.Where(x => x.UserHistoryHeaderId == pParams.UserHistoryHeaderId);
               if(pParams.CategoryRef != null) qry = qry.Where(x => x.CategoryRef == pParams.CategoryRef);
               if(pParams.DateAdded.Year > 2000) qry = qry.Where(x => Convert.ToDateTime(x.CreatedOn) == pParams.DateAdded.Date);
               if(!string.IsNullOrEmpty(pParams.UserName)) qry = qry.Where(x => x.UserName.ToLower()==pParams.UserName.ToLower());
               if(pParams.Status=="concluded") {
                    qry = qry.Where(x =>  x.Status.Contains("concluded"));
               } else if (pParams.Status == "pending") {
                    qry = qry.Where(x =>  !x.Status.ToLower().Contains("concluded") || x.Status==null);
               } else if (!string.IsNullOrEmpty(pParams.Status)) {
                    qry = qry.Where(x => x.Status.ToLower() == pParams.Status.ToLower());
               }
               if(pParams.Id.HasValue) qry = qry.Where(x => x.Id == pParams.Id);
               

               if(pParams.Sort=="name") 
                    {qry = qry.OrderBy(x => x.Name);} 
               else if (pParams.Sort=="categoryref") 
                    {qry = qry.OrderBy(x => x.CategoryRef);} 
               else if (pParams.Sort == "status") 
                    {qry = qry.OrderBy(x => x.Status);}
               else 
                    {qry = qry.OrderBy(x => x.Source).ThenBy(x => x.Name);}

               var totalCount = await qry.CountAsync();
               if(totalCount==0) return null;
               var prospectives = (ICollection<UserHistory>)await qry.Skip((pParams.PageIndex-1)*pParams.PageSize).Take(pParams.PageSize).ToListAsync();
               var userhistorydto = _mapper.Map<ICollection<UserHistory>, ICollection<UserHistoryDto>>(prospectives);
               return new Pagination<UserHistoryDto>(pParams.PageIndex, pParams.PageSize, totalCount, (IReadOnlyList<UserHistoryDto>)userhistorydto);
               
          }


          public async Task<UserHistory> GetOrAddUserHistoryOfProspectiveCandidatesByParams(ProspectiveCandidateParams prospectiveParams)
          {
               //check if the object has empty elements
               
               var cv = new Candidate();
               var history = new UserHistory();
               var histParams = new UserHistoryParams{DateAdded = prospectiveParams.DateAdded, CategoryRef = prospectiveParams.CategoryRef, PersonType = "prospective"};
               history = await GetUserHistory(histParams);

               if (history !=null) return history;     

               //no record in UserHistory, so create new UserHistory after verying input data;
               var userHist = new UserHistory();
               
               var phn = histParams.MobileNo ?? "";
               
               var qryCand = _context.ProspectiveCandidates.AsQueryable();
               if (histParams.PersonId.HasValue) qryCand = qryCand.Where(x => x.Id == histParams.PersonId);
               if (!string.IsNullOrEmpty(histParams.EmailId )) qryCand = qryCand.Where(x => x.Email.ToLower() == histParams.EmailId.ToLower());                         
               if (!string.IsNullOrEmpty(histParams.MobileNo)) qryCand = qryCand.Where(x => x.PhoneNo.Contains(histParams.MobileNo) || x.AlternatePhoneNo.Contains(histParams.MobileNo));
               
               var cand = await qryCand.FirstOrDefaultAsync();
               
               if(cand == null) return null;

               userHist = new UserHistory{PersonType= "prospective", Name = cand.CandidateName, EmailId = cand.Email, CreatedOn = DateTime.Now, MobileNo = cand.PhoneNo};

               _unitOfWork.Repository<UserHistory>().Add(userHist);
               await _unitOfWork.Complete();
                    
               history = await GetUserHistory(histParams);
               if(history==null) return null;

               if(history.UserHistoryItems != null) {
                    foreach(var item in history.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }
               return history;
          }

          public async Task<UserHistoryDto> GetOrAddUserHistoryByParams(UserHistoryParams histParams, string username)
          {
               //check if the object has empty elements
               var returnDto = new UserHistoryDto();

               var qry = _context.UserHistories.Include(x => x.UserHistoryItems) .AsQueryable();
               if (histParams.Id !=0 && histParams.Id != null) qry = qry.Where(x => x.Id==histParams.Id);
               if (histParams.ApplicationNo !=0) qry = qry.Where(x => x.ApplicationNo==histParams.ApplicationNo);
               if (!string.IsNullOrEmpty(histParams.MobileNo)) qry = qry.Where(x => x.MobileNo == histParams.MobileNo);
               if (!string.IsNullOrEmpty(histParams.PersonType)) qry= qry.Where(x => x.PersonType==histParams.PersonType);
               if(!string.IsNullOrEmpty(histParams.PersonName)) qry = qry.Where(x => x.Name.Contains(histParams.PersonName));
               if(!string.IsNullOrEmpty(histParams.Status)) qry = qry.Where(x => x.Status.ToLower()==histParams.Status.ToLower());
               if(!string.IsNullOrEmpty(histParams.UserName)) qry = qry.Where(x => x.UserName.ToLower()==histParams.UserName.ToLower());

               var hist = await qry.FirstOrDefaultAsync();

               if(hist == null) {
                    var candQry = _context.Candidates.Include(x => x.UserPhones).AsQueryable();
                    if(histParams.ApplicationNo != 0) candQry = candQry.Where(x => x.ApplicationNo == histParams.ApplicationNo);
                    if (!string.IsNullOrEmpty(histParams.EmailId)) candQry = candQry.Where(x => x.Email==histParams.EmailId);
                    if (!string.IsNullOrEmpty(histParams.MobileNo)) {
                         var candId = await _context.UserPhones.Where(x => x.MobileNo==histParams.MobileNo).Select(x => x.CandidateId).FirstOrDefaultAsync();
                         candQry = candQry.Where(x => x.Id==candId);
                    }
                    if (!string.IsNullOrEmpty(histParams.UserName)) candQry = candQry.Where(x => x.Email==histParams.UserName);
                    
                    var cand = await candQry.FirstOrDefaultAsync();
                    
                    if(cand==null) {
                         returnDto.ErrorMessage = "Failed to create User History record since no candidte found with criteria provided";
                         return  returnDto;   //look in Prospecives and then in officials
                    }

                    var ph = cand.UserPhones?.Select(x => x.MobileNo).FirstOrDefault();
                    if(string.IsNullOrEmpty(ph)) {
                         returnDto.ErrorMessage="The candidate retrived with the inputs is " + cand.FullName + "; however he does not have any phone no registered";
                         return returnDto;
                    }
                    var src=cand.Source==null ? "N.A." : cand.Source;
                    var cat = cand.UserProfessions?.Select(x => x.Profession).FirstOrDefault();
                    var catName = cat==null ? "" : cat;
                    

                    hist = new UserHistory{PersonType= "candidate", Name = cand.FullName, Source=src,
                         CategoryRef=catName,
                         Address=cand.Address, City=cand.City, Concluded = false, Status="Active",
                         StatusDate = DateTime.Now, UserName = username,
                         EmailId = cand.Email, CreatedOn = DateTime.Now, ApplicationNo = cand.ApplicationNo,
                         MobileNo = ph, PersonId = cand.Id};
                    
                    _unitOfWork.Repository<UserHistory>().Add(hist);
               
                    try{
                         await _unitOfWork.Complete();
                    } catch (Exception ex) {
                         returnDto.ErrorMessage = ex.Message;
                         return returnDto;
                    }
                    
               }

               if(hist==null) return null;
               
               returnDto = _mapper.Map<UserHistory, UserHistoryDto>(hist);

               return returnDto;

               /*if(hist.UserHistoryItems != null) {
                    foreach(var item in hist.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }
               */

          }

          public async Task<UserHistoryItem> UpdateHistoryItem(UserHistoryItem userhistoryitem, string userdisplayname) {
               //ensure personId is valid
               //ensure UserHistoryId is valid
               var appno=0;
               var error="";
               if (userhistoryitem.UserHistoryId==0) {
                    //create new userhistory object
                    if(userhistoryitem.PersonType=="Candidate") {
                         appno = await _context.Candidates.Where(x => x.Id==userhistoryitem.PersonId)
                              .Select(x => x.ApplicationNo).FirstOrDefaultAsync();
                    }
                    var items = new List<UserHistoryItem>();
                    items.Add(userhistoryitem);

                    var userhistory = new UserHistory {
                         CategoryRef=userhistoryitem.CategoryRef,
                         MobileNo=userhistoryitem.PhoneNo,
                         Gender = "M",
                         Source = "",
                         Name = userhistoryitem.PersonName,
                         ApplicationNo = appno,
                         UserName = userdisplayname,
                         CreatedOn = userhistoryitem.DateOfContact,
                         UserHistoryItems=items
                    };
                    _unitOfWork.Repository<UserHistory>().Add(userhistory);
               } else {
                    //assuming client has correct UserhistoryId
                    if(userhistoryitem.Id==0) {
                         _unitOfWork.Repository<UserHistoryItem>().Add(userhistoryitem);
                    } else {
                         _unitOfWork.Repository<UserHistoryItem>().Update(userhistoryitem);
                    }
                    
               }
               
               try {
                    await _unitOfWork.Complete();
               } catch (Exception ex) {
                    error = ex.Message;
                    return null;
               }

               return userhistoryitem;
          }
          public async Task<bool> EditContactHistoryItems(ICollection<UserHistoryItem> model, int loggedinEmpId)
          {
               var existingItems = await _context.UserHistoryItems
                    .Where(x => x.UserHistoryId == model.Select(x => x.UserHistoryId).FirstOrDefault())
                    .AsNoTracking()
                    .ToListAsync();
               
               foreach(var existingItem in existingItems) {
                    if (!model.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.UserHistoryItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               if(model != null) {
                    foreach(var modelItem in model) {
                         var existingItem = existingItems.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                         if(existingItem != null) {
                              _context.Entry(existingItem).CurrentValues.SetValues(existingItem);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {
                              var newItem = new UserHistoryItem(modelItem.IncomingOutgoing, modelItem.UserHistoryId, modelItem.PhoneNo, 
                                   modelItem.DateOfContact.Year < 2000 ? DateTime.Now : modelItem.DateOfContact, 
                                   loggedinEmpId, modelItem.Subject, modelItem.CategoryRef, modelItem.ContactResultId, 
                                   modelItem.ContactResultName, modelItem.PersonName, modelItem.ComposeEmailMessage, modelItem.GistOfDiscussions);
                              
                              existingItems.Add(newItem);
                              _context.Entry(newItem).State = EntityState.Added;
                         }
                    }
               }

               var succeeded = await _context.SaveChangesAsync() > 0;
               return succeeded;

          }

          public async Task<bool> DeleteUserHistoryItem(int UserHistoryItem)
          {
               var historyitemtodelete = await _context.UserHistoryItems.FindAsync(UserHistoryItem);
               if(historyitemtodelete==null) return false;
               _context.UserHistoryItems.Remove(historyitemtodelete);
               _context.Entry(historyitemtodelete).State=EntityState.Deleted;

               return await _context.SaveChangesAsync() > 0;
          }
          public async Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, LoggedInUserDto userDto)
          {
               var historyReturnDto = new UserHistoryReturnDto();

               var existingHistory = await _context.UserHistories
                    .Where(x => x.Id == model.Id)
                    .Include(x => x.UserHistoryItems)
                    //.AsNoTracking()
                    .SingleOrDefaultAsync();
               
               if (existingHistory == null) {
                    historyReturnDto.Succeeded=false;
                    return historyReturnDto;
               }
               //_context.Entry(existingHistory).CurrentValues.SetValues(model);

               existingHistory.Concluded=model.Concluded;
               if ((bool)model.Concluded) {
                    existingHistory.ConcludedById=userDto.LoggedInEmployeeId;
                    existingHistory.ConcludedOn=model.ConcludedOn;
               }

               //save NewHistoryItem for use in composeMessages
               var historyItemForMessage = new UserHistoryItem();
               var historyItemForSMS = new UserHistoryItem();

               foreach(var existingHistoryItem in existingHistory.UserHistoryItems.ToList())
               {
                    if (!model.UserHistoryItems.Any(c => c.Id == existingHistoryItem.Id && 
                         c.Id != default(int)))
                    {
                         _context.UserHistoryItems.Remove(existingHistoryItem);
                         _context.Entry(existingHistoryItem).State = EntityState.Deleted;
                    }
               }

               var historyItemForMessages = new List<UserHistoryItem>();

               if(model.UserHistoryItems != null) {
                    foreach(var modelHistoryItem in model.UserHistoryItems)
                    {
                         var existingModelItem = existingHistory.UserHistoryItems
                              .Where(c => c.Id == modelHistoryItem.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingModelItem != null)
                         {
                              _context.Entry(existingModelItem).CurrentValues.SetValues(modelHistoryItem);
                              _context.Entry(existingModelItem).State = EntityState.Modified;
                         } else {
                              var newHistoryItem = new UserHistoryItem(modelHistoryItem.IncomingOutgoing, model.Id, modelHistoryItem.PhoneNo, 
                                   modelHistoryItem.DateOfContact.Year < 2000 ? DateTime.Now : modelHistoryItem.DateOfContact, 
                                   userDto.LoggedInEmployeeId, modelHistoryItem.Subject, modelHistoryItem.CategoryRef, 
                                   modelHistoryItem.ContactResultId, modelHistoryItem.ContactResultName, 
                                   modelHistoryItem.PersonName, modelHistoryItem.ComposeEmailMessage, modelHistoryItem.GistOfDiscussions);
                                   
                              existingHistory.UserHistoryItems.Add(newHistoryItem);
                              _context.Entry(newHistoryItem).State = EntityState.Added;
                              if(modelHistoryItem.ComposeEmailMessage || modelHistoryItem.ComposeSMS) historyItemForMessages.Add(newHistoryItem);
                         }
                    }
               }

               var mailandsms = new EmailSMSWhatsAppCollectionDto();
               if(historyItemForMessages.Count > 0) {
                    mailandsms = await ComposeMessages(historyItemForMessages, userDto, model.PersonType, model.ResumeId);
                    //in above procedure, the connection is detached, as it is interfering in line 405 below with flg error

                    //_context.Entry<JobConnection>(jobCon).State = EntityState.Detached;  //Explicitly Detach the orphan tracked instance 
                    // The instance of entity type 'UserHistory' cannot be tracked because another instance with the same key value for {'Id'} 
                    //is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. 
               }
               string currentStatus="";
               string username="";
               

               var lastRecord = model.UserHistoryItems.OrderByDescending(x => x.DateOfContact).Take(1)
                    .Select(x => new {x.ContactResultId, x.ContactResultName, x.LoggedInUserName, x.DateOfContact}).FirstOrDefault();
               if (lastRecord != null && string.IsNullOrEmpty(lastRecord.ContactResultName) ) {
                    if(string.IsNullOrEmpty(lastRecord.ContactResultName)) {
                         var temp = await _context.ContactResults.FindAsync(lastRecord.ContactResultId);
                         currentStatus = temp.Name;
                    }
               } else {
                    currentStatus = lastRecord.ContactResultName;
               }
               
               username=lastRecord.LoggedInUserName;
               var dt=lastRecord.DateOfContact;
          
               /* if(model.PersonType.ToLower()=="prospective") {
                    var prospective = await _context.UserHistories.Where(x => x.ResumeId == model.ResumeId).FirstOrDefaultAsync();
                    if(prospective != null) {
                         prospective.Status = currentStatus;
                         prospective.UserName=username;
                         prospective.StatusDate=Convert.ToDateTime(dt);
                         _context.Entry(prospective).State = EntityState.Modified;
                    }
               }
               */
               existingHistory.Status=currentStatus;
               existingHistory.UserName=username;
               existingHistory.StatusDate=dt;

               _context.Entry(existingHistory).State = EntityState.Modified;

               if(mailandsms.EmailMessages !=null && mailandsms.EmailMessages.Count > 0) foreach(var msg in mailandsms.EmailMessages)  _context.Entry(msg).State = EntityState.Added;
               if(mailandsms.SMSMessages !=null && mailandsms.SMSMessages.Count > 0) foreach(var sms in mailandsms.SMSMessages) _context.Entry(sms).State = EntityState.Added;

               historyReturnDto.Succeeded = await _context.SaveChangesAsync() > 0;        //this also deletes the existinghistoryrecord
               int ct = mailandsms.EmailMessages == null ? 0 : mailandsms.EmailMessages.Count;
               int ct2 = mailandsms.SMSMessages == null ? 0 : mailandsms.SMSMessages.Count;
               int ct3 = mailandsms.WhatsAppMessages == null ? 0 : mailandsms.WhatsAppMessages.Count;
               if(historyItemForMessages.Count > 0) historyReturnDto.MessageCount = ct + ct2 +ct3;


               return historyReturnDto;
          }
     
          private async Task<EmailSMSWhatsAppCollectionDto> ComposeMessages(List<UserHistoryItem> historyItemForMessages, LoggedInUserDto userDto, string PersonType, string ResumeId) 
          {
               if (historyItemForMessages.Count == 0) return null;

               var msg = new EmailMessage();
               var sms = new SMSMessage();
               var messages = new List<EmailMessage>();
               var smsMessages = new List<SMSMessage>();
               var modelids = new List<string>();
               var prospectiveObj = new ComposeMessageDtoForProspects();


               //get candidate or prospect variables
               if(PersonType.ToLower()=="prospective")  {
                    var prospect = await _context.UserHistories.Where(x => x.ResumeId == ResumeId).FirstOrDefaultAsync();
                    if(prospect==null) {
                         //returnDto.ErrorMessage="invalid prospective PersonId";
                         return null;
                    }
                    _context.Entry(prospect).State = EntityState.Detached;  //Explicitly Detach the orphan tracked instance 
                         //because it is interfering with another instance of the UserHistries record in main calling program
                         //while saving
                    prospectiveObj = new ComposeMessageDtoForProspects(prospect.Id, prospect.Name, prospect.EmailId, prospect.MobileNo, 
                         prospect.Source, prospect.City, "");
               } else if(PersonType.ToLower()=="candidate") {
                    prospectiveObj = await (from c in _context.Candidates where c.Id == Convert.ToInt32(ResumeId) 
                         join p in _context.UserPhones on c.Id equals p.CandidateId into phones
                         from ph in phones.DefaultIfEmpty()
                         join a in _context.EntityAddresses on c.Id equals a.CandidateId into address 
                         from ad in address.DefaultIfEmpty()
                         select new ComposeMessageDtoForProspects(c.Id, c.FullName, c.Email, ph.MobileNo ?? "",  c.Source, ad.City ?? "", 
                              ad.Country ?? "")).FirstOrDefaultAsync();
               }
          
               
               foreach(var item in historyItemForMessages)
               {
                    if (!item.ComposeEmailMessage) continue;
                    var orderitemdetails = await OrderItemIdFromCategoryRef(item.CategoryRef);
                    if(orderitemdetails == null) continue;
                    if(string.IsNullOrEmpty(prospectiveObj.CategoryName)) prospectiveObj.CategoryName=orderitemdetails.CategoryName;
                    if(string.IsNullOrEmpty(prospectiveObj.City)) prospectiveObj.City=orderitemdetails.City;
                    if(string.IsNullOrEmpty(prospectiveObj.Country)) prospectiveObj.Country=orderitemdetails.Country;

                    modelids.Add(ResumeId);
                    
                    switch(item.ContactResultId) {
                         case (int)EnumContactResult.InterstedOkToSubmithisCV:
                              if(item.ComposeEmailMessage) {
                                   msg = _composeProspectiveMsg.ComposeMessagesForConsentOfInterest(prospectiveObj, userDto);
                                   if (msg != null) messages.Add(msg);
                              }
                              if(item.ComposeSMS) {
                                   sms = _composeProspectiveMsg.ComposeSMSForConsentOfInterest(prospectiveObj,userDto);
                                   if (sms != null) smsMessages.Add(sms);
                              }
                              break;
                         case (int)EnumContactResult.PhoneNotReachable:
                         case (int)EnumContactResult.PhoneUnanswered:
                         case (int)EnumContactResult.PhoneNoWrong:
                         case (int)EnumContactResult.PhoneOutOfNetwork:
                         case (int)EnumContactResult.PhoneAnsweredByOthers:
                              if(item.ComposeEmailMessage) {
                                   msg = _composeProspectiveMsg.ComposeMessagesForFailureToReach(prospectiveObj, userDto);
                                   if(msg != null) messages.Add(msg);
                              }
                              if(item.ComposeSMS) {
                                   sms = _composeProspectiveMsg.ComposeSMSForFailureToReach(prospectiveObj, userDto);
                                   if(sms !=null) smsMessages.Add(sms);
                              }
                              break;
                         case (int)EnumContactResult.DoesNotHavePP:
                              break;
                         case (int)EnumContactResult.NotInterestedForOverseasJob:
                              break;
                         case (int)EnumContactResult.NotInterestedReasonsNotKnown:
                              break;
                         case (int)EnumContactResult.RudeBehavior:
                         case (int)EnumContactResult.SCNotAcceptable:
                         case (int)EnumContactResult.WantsIncreasedSalary:
                         case (int)EnumContactResult.OfferAccepted:
                              break;
                         default:
                              break;
                    }
               
                    
               }
               
               var returnDto = new EmailSMSWhatsAppCollectionDto();
               returnDto.EmailMessages=messages;
               returnDto.SMSMessages=smsMessages;
               return returnDto;
          }

          public async Task<CandidateBriefDto> GetCandidateBriefByParams(CandidateSpecParams SpecParams)
          {
               var cand = new Candidate();

               if(SpecParams.ApplicationNoFrom != 0) {
                    var c = await _context.Candidates.Where(x => x.ApplicationNo == SpecParams.ApplicationNoFrom)
                         .Select(x => new CandidateBriefDto(x.Id, x.Gender, x.ApplicationNo, x.AadharNo, x.FullName,
                              x.City, (int)x.ReferredBy, ""))
                         .FirstOrDefaultAsync();
                    return c;
               } else if (SpecParams.CandidateId != 0) {
                    var c = await _context.Candidates.Where(x => x.Id == SpecParams.CandidateId)
                         .Select(x => new CandidateBriefDto(x.Id, x.Gender, x.ApplicationNo, x.AadharNo, x.FullName,
                              x.City, (int)x.ReferredBy, ""))
                         .FirstOrDefaultAsync();
                    return c;
               } 
               return null;
          }

          public async Task<UserHistory> GetHistoryByResumeId(string ResumeId)
          {
               var history = await _context.UserHistories.Where(x => x.ResumeId == ResumeId )
                    .Include(x => x.UserHistoryItems)
                    .FirstOrDefaultAsync();

               if (history == null) {
                    var cv = await _context.Candidates.Where(x => x.Id == Convert.ToInt32(ResumeId))
                         .Select(x => new {id=x.Id, name=x.FullName, appno=x.ApplicationNo, emailid=x.Email, ph=x.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault()})
                         .FirstOrDefaultAsync();
                    if (cv == null) return null;
                    history = new UserHistory{ApplicationNo=cv.appno, ResumeId=ResumeId, Name=cv.name, PersonType="candidate", MobileNo=cv.ph, EmailId=cv.emailid};
                    _unitOfWork.Repository<UserHistory>().Add(history);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               
               //var historyDto = _mapper.Map<UserHistory, UserHistoryDto>(history);

               if(history.UserHistoryItems != null) {
                    foreach(var item in history.UserHistoryItems) {
                         if (item.LoggedInUserName == "") item.LoggedInUserName = await _commonServices.GetEmployeeNameFromEmployeeId(item.LoggedInUserId);
                         if (item.ContactResultId > 0) item.ContactResultName = Enum.GetName(typeof(EnumContactResult), item.ContactResultId);
                    }
               }

               return history;
          }

          private async Task<OrderItemIdCityCountryDto> OrderItemIdFromCategoryRef(string categoryref)
          {
               if(string.IsNullOrEmpty(categoryref)) return null;
               
               int i = categoryref.IndexOf("-");
               if (i== -1) return null;
               var orderno = categoryref.Substring(0,i);
               var srno = categoryref.Substring(i+1);
               if (string.IsNullOrEmpty(orderno) || string.IsNullOrEmpty(srno)) return null;
               int iorderno = Convert.ToInt32(orderno);
               int isrno = Convert.ToInt32(srno);

               var qry = await (from o in _context.Orders where o.OrderNo == iorderno 
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join item in _context.OrderItems on o.Id equals item.OrderId where item.SrNo==isrno
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    select new OrderItemIdCityCountryDto{OrderItemId=item.Id, CategoryName=cat.Name, 
                    City=c.City, Country=c.Country}).FirstOrDefaultAsync();
               if(qry==null) return null;
               return qry;
          }

          public async Task<ICollection<CategoryRefDto>> GetCategoryRefDetails()
          {
               var qry = await (from i in _context.OrderItems where i.Status.ToLower() != "concluded"
                    join o in _context.Orders on i.OrderId equals o.Id 
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new CategoryRefDto {
                         OrderNo = o.OrderNo, SrNo=i.SrNo, 
                         CategoryRef = o.OrderNo + "-" + i.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName
                    }).ToListAsync();
               
               return qry;
          }
     }
}