using System.Text.RegularExpressions;
using System.Xml.Xsl;
using core.Dtos;
using core.Entities.Attachments;
using core.Entities.HR;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class UserGetAndUpdateService: IUserGetAndUpdateService
    {
        private readonly ATSContext _context;
        private readonly IUnitOfWork _unitOfWork;
		public UserGetAndUpdateService(ATSContext context, IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
            _context = context;
		}

          public async Task<string> SaveUploadedFiles(ICollection<FileUpload> filesuploaded) 
          {
               foreach(var f in filesuploaded)
               {
                    _unitOfWork.Repository<FileUpload>().Add(f);
               }

               try {
                    await _unitOfWork.Complete();
               } catch (Exception ex) {
                    return ex.Message;
               } 
               
               return "";
               
          }

          /* 
          private async Task<int> UpdateUserProfessionsWhereNull() {
               
               int ret=0;

               var UserProfWithNullName = await _context.UserProfessions.Where(x => string.IsNullOrEmpty(x.Profession)).ToListAsync();
               var profs = await _context.Categories.Where(x => UserProfWithNullName.Select(x => x.CategoryId).Contains(x.Id)).ToListAsync();
               foreach(var cat in UserProfWithNullName) {
                    cat.Profession = profs.Where(x => x.Id==cat.CategoryId).Select(x => x.Name).FirstOrDefault();
                    _context.Entry(cat).State=EntityState.Modified;
               }
               if(_context.ChangeTracker.HasChanges()) {
                    if(await _context.SaveChangesAsync() > 0) ret = nextRandom();
               } else {
                    ret = nextRandom();
               }

               return ret;
          }
          */
          
          private int nextRandom(){
               Random rnd = new Random();
               return rnd.Next(1,1000);
          }
          public async Task<Pagination<CandidateBriefDto>> GetCandidateBriefPaginated(CandidateSpecParams prm)
          {
               
               var brief = (from c in _context.Candidates where c.CandidateStatus !=(int)EnumCandidateStatus.NotAvailable         //notavailable=500
                    //join cl in _context.Customers on c.ReferredBy equals cl.Id into cust
                    //from customers in cust.DefaultIfEmpty()
                    //join professions in _context.UserProfessions on c.Id equals professions.CandidateId into prof 
                    //from p in prof.DefaultIfEmpty()
                    orderby c.ApplicationNo 
                    select new CandidateBriefDto{
                         Id = c.Id, FullName = c.FullName, City = c.City, ApplicationNo = c.ApplicationNo, 
                         ReferredById=(int)c.ReferredBy     //, ReferredByName= customers.CustomerName
                    }).AsQueryable();

               if(prm.ProfessionId.HasValue) {
                    var candidateIds = await _context.UserProfessions.Where(x => x.CategoryId==prm.ProfessionId).Select(x => x.CandidateId).ToListAsync();
                    brief = brief.Where(x => candidateIds.Contains(x.Id));
               }

               if(!string.IsNullOrEmpty(prm.Search)) {
                    var appnoslist = new List<int>();
                    var namelist = new List<string>();
                    string[] searchString = prm.Search.Split(",");
                    for(int i=0; i < searchString.Length; i++) {
                         var s = searchString[i].Trim();
                         if(Regex.IsMatch(s, @"^\d+$")){
                              appnoslist.Add(Convert.ToInt32(s));
                         } else {
                              namelist.Add(s.ToLower());
                         }
                    }

                    if(appnoslist.Count > 0) {
                         brief = brief.Where(x => appnoslist.Contains(x.ApplicationNo));
                    } else {
                         brief = brief.Where(x => namelist.Contains(x.FullName.ToLower()));
                    }
               }
               var count = await brief.CountAsync();
               var dt = await brief.ToListAsync();
               if (count==0) return null;
               
               var dto = await brief.Skip((prm.PageIndex-1)*prm.PageSize).Take(prm.PageSize).ToListAsync();
               
               //now, find agents and proessions to populae the DTO
               var officialList = dto.Select(x => x.ReferredById).Distinct().ToList();
               var offAndCust = await (from off in _context.CustomerOfficials where officialList.Contains(off.Id)
                    join cust in _context.Customers on off.CustomerId equals cust.Id 
                    select new {OfficialId = off.Id, CustomerName = cust.KnownAs})
                    .ToListAsync();

               var UserProfs = await _context.UserProfessions.Where(x => string.IsNullOrEmpty(x.Profession)).ToListAsync();
               var cats = await _context.Categories.Where(x => UserProfs.Select(x => x.CategoryId).ToList().Contains(x.Id)).ToListAsync();
               foreach(var prof in UserProfs) {
                    var cat = cats.Find(x => x.Id == prof.CategoryId);
                    if(cat != null) {
                         prof.Profession=cat.Name;
                         _context.Entry(prof).State=EntityState.Modified;
                    }
               }
               if(_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();

               var prosfs = await _context.UserProfessions.Where(x => dto.Select(y => y.Id).ToList().Contains(x.CandidateId)).ToListAsync();

               foreach(var item in dto) {
                    var proflist = new List<UserProfession>();
                    var filteredprof = UserProfs.Where(x => x.CandidateId==item.Id).ToList();
                    item.UserProfessions = filteredprof;
                    var nm = offAndCust.Find(x => x.OfficialId==item.ReferredById);
                    if (nm != null) item.ReferredByName = nm.CustomerName;
               }
               var pag = new Pagination<CandidateBriefDto>(prm.PageIndex, prm.PageSize, count, dto);

               return pag;
          }
          
          public async Task<string> DeleteUploadedFile(FileUpload fileupload)
          {
               var fileWithPathName = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded\\Files", fileupload.Name);
               try {
                    System.IO.File.Delete(fileWithPathName);
                    //update DB
                    _unitOfWork.Repository<FileUpload>().Delete(fileupload);
                    await _unitOfWork.Complete();
                    return "";
               } catch (Exception ex) {
                    return ex.Message;
               }
          }
          
          public async Task<ICollection<CandidateBriefDto>> GetCandidateListBrief(CandidateSpecParams prm)
          {
               var qry = _context.Candidates.AsQueryable();
               
               if(!string.IsNullOrEmpty(prm.Mobile)) {
                    var contactPhoneId = await _context.UserPhones.Where(x => x.MobileNo==prm.Mobile).Select(x => x.CandidateId).ToListAsync();
                    qry = qry.Where(x => contactPhoneId.Contains(x.Id)); 
               } 

               if(prm.ProfessionId!=0) {
                    var contactProfIds = await _context.UserProfessions.Where(x => x.CategoryId==prm.ProfessionId).Select(x => x.CandidateId).ToListAsync();
                    qry = qry.Where(x => contactProfIds.Contains(x.Id)); 
               }

               var brief = (from c in _context.Candidates where c.CandidateStatus !=(int)EnumCandidateStatus.NotAvailable
                    join cl in _context.Customers on c.ReferredBy equals cl.Id 
                    select new CandidateBriefDto{
                         Id = c.Id, FullName = c.FullName, City = c.City, ApplicationNo = c.ApplicationNo, 
                         ReferredById=(int)c.ReferredBy, ReferredByName= cl.CustomerName, UserProfessions = c.UserProfessions
                    }).AsQueryable();
               var count = await brief.CountAsync();
               if (count==0) return null;
               

               if(prm.PageSize > 0) {
                    return await brief.Skip(prm.PageIndex-1).Take(prm.PageSize).ToListAsync();
               } else {
                    return await brief.ToListAsync();
               }
          }
          
		public async Task<Pagination<Candidate>> GetCandidates(CandidateSpecParams candidateParams)
          {
               
               var specsParams = new CandidateSpecs(candidateParams);
               var countParams = new CandidateForCountSpecs(candidateParams);

               var totalItems = await _unitOfWork.Repository<Candidate>().CountAsync(countParams);
               var candidateList = await _unitOfWork.Repository<Candidate>().ListAsync(specsParams);

               return new Pagination<Candidate>(candidateParams.PageIndex, candidateParams.PageSize, totalItems, candidateList);
          }

        public async Task<Candidate> GetCandidateByIdWithAllIncludes(int id)
          {
               return await _context.Candidates.Where(x => x.Id == id)
                    .Include(x => x.UserPhones)
                    .Include(x => x.UserQualifications)
                    //.Include(x => x.EntityAddresses)
                    //.Include(x => x.UserPassports)
                    .Include(x => x .UserAttachments)
                    .Include(x => x.UserExperiences)
                    .Include(x => x.UserProfessions)
               .FirstOrDefaultAsync();
          }
        public async Task<ICollection<Candidate>> GetCandidatesWithProfessions(CandidateSpecParams param)
          {
               var query = _context.Candidates.AsQueryable();

               if (param.ApplicationNoFrom.HasValue && param.ApplicationNoUpto.HasValue)
                    query = query.Where(x => x.ApplicationNo >= param.ApplicationNoFrom && 
                         x.ApplicationNo <= param.ApplicationNoUpto);
               
               if (param.AgentId.HasValue) query = query.Where(x => x.CompanyId == param.AgentId);
               
               if (param.ProfessionId.HasValue)
               {
                    var candidateIds = await _context.UserProfessions.Where(x => x.CategoryId == param.ProfessionId).Select(x => x.CandidateId).Distinct().ToListAsync();
                    query = query.Where(x => candidateIds.Contains(x.Id));
               }

               if (param.RegisteredFrom.HasValue)  {
                    if (param.RegisteredUpto.HasValue) {
                         query = query.Where(x => 
                              (DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredFrom)) <= 0)
                              && (DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredUpto)) >=0));
                    } else {
                         query = query.Where(x => 
                              DateTime.Compare(x.Created, Convert.ToDateTime(param.RegisteredFrom)) < 1);
                    }
               }
               
               if (param.IncludeUserProfessions) query = query.Include(x => x.UserProfessions);
               
               //var qry = await query.ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider).ToListAsync();
               return await query.ToListAsync();
               //return new Pagination<CandidateBriefDto>(param.PageIndex, param.PageSize, qry.Count(), qry);

          }
          
        public async Task<ICollection<UserAttachment>> UpdateUserAttachments(ICollection<UserAttachment> model) {

               int candidateid = model.Select(X => X.CandidateId).FirstOrDefault();
               var existingAttachments = await _context.UserAttachments
                    .Where(x => x.CandidateId == candidateid).AsNoTracking().ToListAsync();

               //delete records that exist in DB but not in the model - this happens when
               //the user has deleted records and those records do not reflect in the model
               if(existingAttachments !=null && existingAttachments.Count > 0) {
                    foreach(var existingItem in existingAttachments.ToList()) {
                         if(!model.Any(c => c.Id == existingItem.Id && c.Id != default(int))) {
                              _context.UserAttachments.Remove(existingItem);
                              _context.Entry(existingItem).State = EntityState.Deleted;
                         }
                    }
                    foreach(var item in model) {
                         var existingItem = existingAttachments.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if(existingItem !=null) {     //edit
                              if(item.CandidateId ==0) item.CandidateId=candidateid;
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {       //add new
                              var newObj = new UserAttachment(item.AppUserId, candidateid, item.FileName, item.AttachmentType,
                                   item.AttachmentSizeInBytes, item.url, item.UploadedByEmployeeId, item.DateUploaded);
                              _context.Entry(item).State=EntityState.Added;
                         }
                    }
                    if(existingAttachments==null) {
                         _context.Entry(existingAttachments).State=EntityState.Added;
                    } else {
                         _context.Entry(existingAttachments).State=EntityState.Modified;
                    }

               } else if ((existingAttachments == null || existingAttachments.Count == 0) && (model != null || model.Count > 0)) {
                    //NO EXISTING RECORD, but model has records, so all of the model is to be added
                    foreach(var item in model) {
                         var newObj = new UserAttachment(item.AppUserId, candidateid, item.FileName, item.AttachmentType,
                              item.AttachmentSizeInBytes, item.url, item.UploadedByEmployeeId, item.DateUploaded);
                         _context.Entry(item).State=EntityState.Added;
                    }
               }
               if (await _context.SaveChangesAsync() > 0) {
                    return existingAttachments == null ? model : existingAttachments;
               }
               return null;
          }
          
        public async Task<CandidateWithNewAttachmentDto> UpdateCandidateAsync(Candidate model )
          {
               var existingObject = await _context.Candidates.Where(x => x.Id == model.Id)
                    //.Include(x => x.EntityAddresses)
                    .Include(x => x.UserPhones)
                    .Include(x => x.UserQualifications)
                    .Include(x => x.UserProfessions)
                    .Include(x => x.UserExperiences)
                    .Include(x => x.UserAttachments)
                    //.Include(x => x.UserPassports)
                    .AsNoTracking()
               .FirstOrDefaultAsync();

               if (existingObject == null) return null;
               //update top level entity, i.e. candidate, without related obj

               _context.Entry(existingObject).CurrentValues.SetValues(model);

               //start updating related entities
               //start with deleting records from DB whicha are not present in the model
               //UserPhones
               if (model.UserPhones != null && model.UserPhones.Count > 0 ) {
                    if(existingObject.UserPhones != null) {
                         foreach(var existingItem in existingObject.UserPhones.ToList()) {
                              if (!model.UserPhones.Any(c => c.Id == existingItem.Id && c.Id != default(int))) {
                                   _context.UserPhones.Remove(existingItem);
                                   _context.Entry(existingItem).State = EntityState.Deleted;
                              }
                         }
                    }
                    foreach(var item in model.UserPhones) {
                         var existingItem = existingObject.UserPhones.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null)     //record present in DB, therefore update DB record with values from the model
                         {
                              if (item.CandidateId == 0) item.CandidateId = model.Id;
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {        //insert new record
                              var newObj = new UserPhone(existingObject.Id, item.MobileNo, item.IsMain);
                              existingObject.UserPhones.Add(newObj);
                              _context.Entry(newObj).State = EntityState.Added;
                         }
                    }
               }

               //UserQualifications
               if(model.UserQualifications != null && model.UserQualifications.Count > 0) {
                    if (existingObject.UserQualifications != null) {
                         foreach(var existingQItem in existingObject.UserQualifications.ToList()) {
                              if (!model.UserQualifications.Any(c => c.Id == existingQItem.Id && c.Id != default(int)))
                              {
                                   _context.UserQualifications.Remove(existingQItem);
                                   _context.Entry(existingQItem).State = EntityState.Deleted;
                              }
                         }
                         foreach(var itemQ in model.UserQualifications) {
                              if (itemQ.CandidateId == 0) itemQ.CandidateId = model.Id;
                              var existingQItem = existingObject.UserQualifications.Where(c => c.Id == itemQ.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingQItem != null) {    //record present in DB, therefore update DB record with values from the model
                                   _context.Entry(existingQItem).CurrentValues.SetValues(itemQ);
                                   _context.Entry(existingQItem).State = EntityState.Modified;
                              } else {         //insert new record
                                   var newQObj = new UserQualification(existingObject.Id, itemQ.QualificationId, itemQ.IsMain);
                                   existingObject.UserQualifications.Add(newQObj);
                                   _context.Entry(newQObj).State = EntityState.Added;
                              }
                         }
                    }
               }

               //UserProfessions
               if(model.UserProfessions != null && model.UserProfessions.Count > 0) {
                    if (existingObject.UserProfessions != null) {
                         foreach(var existingPItem in existingObject.UserProfessions.ToList())
                         {
                              if (!model.UserProfessions.Any(c => c.Id == existingPItem.Id && c.Id != default(int)))
                              {
                                   _context.UserProfessions.Remove(existingPItem);
                                   _context.Entry(existingPItem).State = EntityState.Deleted;
                              }
                         }
                         foreach(var itemP in model.UserProfessions) {
                              if (itemP.CandidateId == 0) itemP.CandidateId = model.Id;
                              var existingPItem = existingObject.UserProfessions.Where(c => c.Id == itemP.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingPItem != null) {    //record present in DB, therefore update DB record with values from the model
                                   _context.Entry(existingPItem).CurrentValues.SetValues(itemP);
                                   _context.Entry(existingPItem).State = EntityState.Modified;
                              } else {         //insert new record
                                   var newObjP = new UserProfession(existingObject.Id, itemP.CategoryId, itemP.IndustryId, itemP.IsMain);
                                   existingObject.UserProfessions.Add(newObjP);
                                   _context.Entry(newObjP).State = EntityState.Added;
                              }
                         }
                    }
               }

               //UserAExperiences
               if (model.UserExperiences != null && model.UserExperiences.Count > 0) {
                    foreach(var existingEItem in existingObject.UserExperiences.ToList()) {
                         if (!model.UserExperiences.Any(c => c.Id == existingEItem.Id && c.Id != default(int)))
                         {
                              _context.UserExps.Remove(existingEItem);
                              _context.Entry(existingEItem).State = EntityState.Deleted;
                         }
                    }
                    foreach(var itemE in model.UserExperiences) {
                         if (itemE.CandidateId == 0) itemE.CandidateId = model.Id;
                         var existingEItem = existingObject.UserExperiences.Where(c => c.Id == itemE.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingEItem != null) {    //record present in DB, therefore update DB record with values from the model
                              _context.Entry(existingEItem).CurrentValues.SetValues(itemE);
                              _context.Entry(existingEItem).State = EntityState.Modified;
                         } else {         //insert new record
                              var nextSrNo = (await _context.UserExps.Where(x => x.CandidateId == existingObject.Id).MaxAsync(x => (int?)x.SrNo)) ?? 0;
                              ++nextSrNo;
                              var newObjE = new UserExp(existingObject.Id, nextSrNo, (int)itemE.PositionId, itemE.Employer, 
                                   itemE.Position, itemE.SalaryCurrency, (int)itemE.MonthlySalaryDrawn, itemE.WorkedFrom, 
                                   (DateTime)itemE.WorkedUpto);
                              existingObject.UserExperiences.Add(newObjE);
                              _context.Entry(newObjE).State = EntityState.Added;
                         }
                    }
               }
               
               //UserAttachments
               var fileDirectory = Directory.GetCurrentDirectory();

               List<string>  attachmentsToDelete = new List<string>();          //lsit of files to delete physically from the api space
               List<UserAttachment> attachmentsToAdd = new List<UserAttachment>();

               //if (model.UserAttachments != null && model.UserAttachments.Count > 0) {
                    foreach(var existingAItem in existingObject.UserAttachments.ToList()) {
                         if (!model.UserAttachments.Any(c => c.Id == existingAItem.Id && c.Id != default(int)))
                         {
                              _context.UserAttachments.Remove(existingAItem);
                              _context.Entry(existingAItem).State = EntityState.Deleted;
                              var filepath = existingAItem.url?? fileDirectory + "/assets/images";
                              attachmentsToDelete.Add(filepath + "/" + existingAItem.FileName);        //save file nams to delete later
                         }
                    }
                    foreach(var item in model.UserAttachments) {
                         if (item.CandidateId == 0) item.CandidateId = model.Id;
                         var existingItem = existingObject.UserAttachments.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null) {    //record present in DB, therefore update DB record with values from the model
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {         //insert new record
                              var newObj = new UserAttachment(item.AppUserId, model.Id, item.FileName, item.AttachmentType, 
                                   item.AttachmentSizeInBytes, item.url, item.UploadedByEmployeeId, DateTime.UtcNow);
                              existingObject.UserAttachments.Add(newObj);
                              _context.Entry(newObj).State = EntityState.Added;
                              attachmentsToAdd.Add(item);
                         }
                    }
               //}
               
               _context.Entry(existingObject).State = EntityState.Modified;
               
               int recordsAffected = 0;
               try {
                    recordsAffected = await _context.SaveChangesAsync();
                    //return existingObject;
               } catch (System.Exception ex) {
                    Console.Write(ex.Message);
                    return null;
               }

               if(recordsAffected > 0 && attachmentsToDelete.Count>0) {
                    do {
                         try {
                              File.Delete(attachmentsToDelete[attachmentsToDelete.Count]);
                         } catch (System.Exception ex) {
                              Console.Write(ex.Message);
                         }
                    } while (attachmentsToDelete.Count > 0);
               }

               return new CandidateWithNewAttachmentDto {Candidate = existingObject, NewAttachments=attachmentsToAdd};
          }

        
        public async Task<CandidateBriefDto> GetCandidateByAppNo(int appno)
          {
               var cv = await _context.Candidates.Where(x => x.ApplicationNo == appno)
                    .Select(x => new CandidateBriefDto{
                         Id = x.Id, Gender = x.Gender, ApplicationNo = appno, 
                         FullName = x.FullName, City = x.City, ReferredById =(int)x.ReferredBy,
                         AadharNo = x.AadharNo,
                         CandidateStatusName = Enum.GetName(typeof(EnumCandidateStatus), x.CandidateStatus)})
                    .FirstOrDefaultAsync();
               return cv;
          }

        public async Task<CandidateBriefDto> GetCandidateBriefById(int candidateid)
          {
               var cv = await _context.Candidates.Where(x => x.Id == candidateid)
                    .Select(x => new CandidateBriefDto{
                         Id = x.Id, Gender = x.Gender, ApplicationNo = x.ApplicationNo, 
                         FullName = x.FullName, City = x.City, ReferredById = (int)x.ReferredBy,
                    })
                    .FirstOrDefaultAsync();
               return cv;
          }

    }
}