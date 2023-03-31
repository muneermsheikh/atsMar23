using AutoMapper;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Params;
using core.Dtos;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using core.Entities.MasterEntities;

namespace infra.Services
{
     public class UserService : IUserService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly UserManager<AppUser> _userManager;
          private readonly ITaskService _taskService;
          private readonly IComposeMessagesForAdmin _composeMsgAdmin;
          private readonly IComposeMessagesForHR _composeMsgHR;
          private readonly IConfiguration _config;
          private readonly IMapper _mapper;
          //private readonly TokenService _tokenService;
          public UserService(IUnitOfWork unitOfWork, ATSContext context, ITaskService taskService, IConfiguration config,
               UserManager<AppUser> usermanager, IComposeMessagesForAdmin composeMsgAdmin, IComposeMessagesForHR composeMsgHR,
               IMapper mapper  
               //, TokenService tokenService
               )
          {
               //_tokenService = tokenService;
               _composeMsgHR = composeMsgHR;
               _userManager = usermanager;
               _context = context;
               _unitOfWork = unitOfWork;
               _taskService = taskService;
               _composeMsgAdmin = composeMsgAdmin;
               _config = config;
               _mapper = mapper;
               //_logger = logger;
          }


          public async Task<Candidate> CreateCandidateObject(RegisterDto registerDto, int loggedInEmployeeId)
          {
               var NextAppNo = await _context.Candidates.MaxAsync(x => x.ApplicationNo);
               NextAppNo = NextAppNo == 0 ? 10001 : NextAppNo+1;
               //registerDto.Address is not forwarded by client, but is populated here from the collection EntityAddresses
               var cand = new Candidate(registerDto.Gender, registerDto.AppUserId, 
                    registerDto.AppUserIdNotEnforced ? registerDto.AppUserIdNotEnforced : true,
                    NextAppNo, registerDto.FirstName, registerDto.SecondName ?? "", registerDto.FamilyName ?? "", registerDto.DisplayName, 
                    registerDto.DOB, registerDto.PlaceOfBirth ??"", registerDto.AadharNo??"", registerDto.Email, registerDto.Introduction ?? "",
                    registerDto.Interests ?? "", registerDto.NotificationDesired, registerDto.Nationality, registerDto.CompanyId , 
                    registerDto.PpNo ?? "", registerDto.City, registerDto.Pin,
                    registerDto.ReferredBy,
                    registerDto.UserQualifications, registerDto.UserProfessions,
                    null);
               
               cand.Created = DateTime.UtcNow;
               cand.LastActive = DateTime.UtcNow;

               //PP No is unique in the db - include only those passports that do not already exist in the database
               var lstPP = new List<UserPassport>();
               if (cand.UserPassports != null) {
                    foreach (var pp in cand.UserPassports)
                    {
                         var existingPP = await _context.UserPassports.Where(c => c.PassportNo == pp.PassportNo).FirstOrDefaultAsync();
                         if (existingPP == null) lstPP.Add(pp);
                    }
               }
               
               cand.UserPassports = lstPP;

               var lstP = new List<UserPhone>();
               if (registerDto.UserPhones != null && registerDto.UserPhones.Count > 0 )cand.UserPhones = registerDto.UserPhones;

               if (registerDto.UserProfessions != null  && registerDto.UserProfessions.Count > 0) cand.UserProfessions = registerDto.UserProfessions;
               if (registerDto.UserQualifications != null && registerDto.UserQualifications.Count > 0) cand.UserQualifications=registerDto.UserQualifications;
               if (registerDto.UserAttachments != null && registerDto.UserAttachments.Count > 0) {
                    foreach(var att in registerDto.UserAttachments) {
                         att.FileName = NextAppNo + "-" + att.FileName;
                         att.url = Directory.GetCurrentDirectory();
                         att.UploadedByEmployeeId = loggedInEmployeeId;
                         //   attachmentType is already set
                         if(att.AttachmentType.ToLower()=="photograph") {
                              cand.PhotoUrl=att.url + "/" + att.FileName;
                         }
                    }
                    cand.UserAttachments = registerDto.UserAttachments;
               }
               return cand;
          }
          public async Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, int loggedInEmployeeId)
          {
               var cand = await CreateCandidateObject(registerDto, loggedInEmployeeId);

               _unitOfWork.Repository<Candidate>().Add(cand);

               if (registerDto.NotificationDesired) {
                    var paramsDto = new CandidateMessageParamDto{Candidate = cand, DirectlySendMessage = false};
                    await _composeMsgHR.ComposeHTMLToAckToCandidateByEmail(paramsDto);
                    //await _composeMsgHR.ComposeMsgToAckToCandidateBySMS(paramsDto);
               }
               
               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               return cand;
          }

          public async Task<bool> UpdateCandidatePhoto(int CandidateId, string photoUrl) 
          {
               var candidate = await _context.Candidates.FindAsync(CandidateId);

               if (candidate==null) return false;

               candidate.PhotoUrl=photoUrl;

               _context.Candidates.Update(candidate);

               return await _context.SaveChangesAsync() > 0;
               
          }

     //employees
          public async Task<Employee> CreateEmployeeAsync(RegisterEmployeeDto registerDto)
          {
               /*
               //CHECK IF PHONES ALREADY TAKEN
               */

               var qs = new List<EmployeeQualification>();
               if(registerDto.EmployeeQualifications !=null && registerDto.EmployeeQualifications.Count > 0)
               {
                    foreach(var q in registerDto.EmployeeQualifications)
                    {
                         qs.Add(new EmployeeQualification(q.QualificationId, q.IsMain));
                    }
               }

               var phs = new List<EmployeePhone>();
               if(registerDto.EmployeePhones != null && registerDto.EmployeePhones.Count() > 0) {
                    foreach(var p in registerDto.EmployeePhones)
                    {
                         phs.Add(new EmployeePhone(p.MobileNo, p.IsMain));
                    }
               }
               phs = phs.Count() > 0 ? phs : null;

               var hrskills = new List<EmployeeHRSkill>();
               if(registerDto.HrSkills!=null && registerDto.HrSkills.Count > 0) {
                    foreach(var h in registerDto.HrSkills) {
                         hrskills.Add(new EmployeeHRSkill(h.CategoryId,h.IndustryId, h.SkillLevel));
                    }
               }
               hrskills = hrskills.Count() > 0 ? hrskills : null;

               var otherskills = new List<EmployeeOtherSkill>();
               if(registerDto.OtherSkills != null && registerDto.OtherSkills.Count() > 0) {
                    foreach(var o in registerDto.OtherSkills) {
                         otherskills.Add(new EmployeeOtherSkill(o.SkillDataId, o.SkillLevel, o.IsMain));
                    }
               }
               otherskills = otherskills.Count() > 0 ? otherskills: null;
               
               var employeeAddresses = new List<EmployeeAddress>();
               if (registerDto.EmployeeAddresses!=null && registerDto.EmployeeAddresses.Count > 0) {
                    foreach(var a in registerDto.EmployeeAddresses) {
                         employeeAddresses.Add(new EmployeeAddress(a.AddressType, a.Add, a.StreetAdd,
                         a.City, a.Pin, a.State, a.District, a.Country, a.IsMain));
                    }
               }
               
               var emp = new Employee(registerDto.AppUserId, registerDto.Gender, registerDto.FirstName,
                    registerDto.SecondName, registerDto.FamilyName, registerDto.DisplayName,
                    registerDto.AadharNo, registerDto.DateOfBirth, registerDto.DateOfJoining, registerDto.Department,
                    registerDto.Position, registerDto.Email,  qs, hrskills, otherskills, employeeAddresses);

               _unitOfWork.Repository<Employee>().Add(emp);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               /*if (!await CreateAppUser("employee", registerDto.DisplayName, registerDto.Email, registerDto.Password))
                    throw new Exception("Exception Code 400 - bad request - failed to create the identity user");
               */
               return emp;
          }

          public async Task<CustomerOfficial> CreateCustomerOfficialAsync(RegisterDto registerDto)
          {
               var off = new CustomerOfficial(registerDto.Gender, registerDto.Position, 
                    registerDto.FirstName + " " + registerDto.SecondName + " " + registerDto.FamilyName, registerDto.Position, 
                    registerDto.UserPhones.Where(x => !string.IsNullOrEmpty(x.MobileNo)).Select(x => x.MobileNo).FirstOrDefault(),  
                    registerDto.UserPhones.Where(x => x.IsMain && !string.IsNullOrEmpty(x.MobileNo)).Select(x => x.MobileNo).FirstOrDefault(),
                    registerDto.Email, "", true);
                    
               _unitOfWork.Repository<CustomerOfficial>().Add(off);

               var result = await _unitOfWork.Complete();

               if (result <= 0) return null;

               if (!await CreateAppUser("official", registerDto.DisplayName, registerDto.Email, registerDto.Password)) 
                    throw new Exception("Exception Code 400 - bad request - failed to create the identity user");

               return off;
          }

          private async Task<bool> CreateAppUser(string usertype, string displayname, string email, string password)
          {
               var user = new AppUser
               {
                    UserType = usertype,
                    DisplayName = displayname,
                    Email = email,
                    UserName = email
               };

               var succeeded = await _userManager.CreateAsync(user, password);

               return succeeded.Succeeded;

          }
          public async Task<AppUser> CreateUserAsync(RegisterDto dto)
          {
               //for customer and vendor official, customer Id is mandatory
               if (dto.UserType.ToLower() =="employee" && string.IsNullOrEmpty(dto.AadharNo)) {
                    throw new Exception("Exception Code 400 - for employees, Aadhar number is mandatory");
                    //return null;
               }

               if (dto.UserType.ToLower()=="official" && (int)dto.CompanyId==0) {
                    throw new Exception ("Error 400 - For officials, customer Id is essential");
                    //return null;
               }
               
               if (dto.UserPhones !=null && dto.UserPhones.Count() > 0) {
                    foreach(var ph in dto.UserPhones) {
                         if (ph.MobileNo == "") return null;     // BadRequest(new ApiResponse(400, "either phone no or mobile no must be mentioned"));
                    }
               }

               var objPP = new UserPassport();

               if(string.IsNullOrEmpty(dto.PpNo)) {
                    objPP = null;
               } else {
                    objPP = new UserPassport(dto.PpNo, dto.Nationality, (DateTime)dto.PPValidity);
               }

               var user = new AppUser
               {
                    UserType = dto.UserType,
                    DisplayName = dto.DisplayName,
                    //Address = dto.Address,
                    //UserPassport = objPP,

                    Email = dto.Email,
                    UserName = dto.Email
               };

               var result = await _userManager.CreateAsync(user, dto.Password);

               if (!result.Succeeded) throw new Exception("Exception Code 400 - bad request - failed to create the identity user");

               var userDtoToReturn = new UserDto
               {
                    DisplayName = user.DisplayName,
                    //Token = _tokenService.CreateToken(user),
                    Email = user.Email
               };

               //var userAdded = await _userManager.FindByEmailAsync(dto.Email);
               return user;
          }

          public async Task<bool> CheckEmailExistsAsync(string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }
          
          public async Task<bool> CheckCandidateAadharNoExists(string aadharNo)
          {
               var cand = await _context.Candidates.Where(x => x.AadharNo == aadharNo).FirstOrDefaultAsync();
               return (cand != null);
          }

          public async Task<bool> CheckAadharNoExists(string aadharNo)
          {
               var emp = await _context.Employees.Where(x => x.AadharNo == aadharNo).FirstOrDefaultAsync();
               return (emp != null);
          }

          public async Task<ICollection<UserProfession>> EditUserProfessions(UserAndProfessions userProfessions)
          {
               var candidateId = userProfessions.CandidateId;

               var selectedProfessions = new List<UserProfession>();
               foreach(var item in userProfessions.CandidateProfessions)
               {
                    if (await _unitOfWork.Repository<Category>().GetByIdAsync(item.CategoryId) != null)  
                         // check if the profession exists in DB before adding to selectedProfessions
                         selectedProfessions.Add(new UserProfession(
                         candidateId, item.CategoryId, item.IndustryId, item.IsMain));
               } 

               if (selectedProfessions.Count() == 0 ) 
                    throw new Exception ("none of the professions of the Candidate exist on record");
                    
               //get the candidate Professions as exist in DB.  Add to the DB those professions that do not
               //existing in this DB set (selectedProfessions.ExceptWhatExist in Db)
               var existingUserProfessions = await _unitOfWork.Repository<UserProfession>()
                    .ListAsync(new UserProfessionsSpecs(candidateId));
                         //
               var selectedCategoryIds = userProfessions.CandidateProfessions.Select(x => x.CategoryId).ToArray();
               var existingUserCategoryIds = existingUserProfessions.Select(x => x.CategoryId).ToList();
               foreach(var p in selectedProfessions.Where(x => !existingUserCategoryIds.Contains(x.CategoryId)).ToList())
               {
                    _unitOfWork.Repository<UserProfession>().Add(new UserProfession(candidateId, p.CategoryId, p.IndustryId, p.IsMain));
               }
               
               //remove professions
               //int ct = 0;
               //var selectedIds = selectedProfessions.Select(x => x.CategoryId).ToList();
               foreach(var p in existingUserProfessions.Where(x => !selectedProfessions.Select(x => x.CategoryId).ToList().Contains(x.CategoryId)).ToList())
               {
                    _unitOfWork.Repository<UserProfession>().Delete(p);
                    //ct++;
               }
               
               if(await _unitOfWork.Complete() == 0) throw new Exception("Failed to edit the user professions, Or the records did not need to change");

               return selectedProfessions;
          }

          public async Task<ICollection<CandidateCity>> GetCandidateCityNames()
          {
               var c = await _context.Candidates
                    .Select(x => x.City).Distinct() .ToListAsync();
               var lsts = new List<CandidateCity>();
               foreach(var lst in c)
               {
                    lsts.Add(new CandidateCity{City = lst});
               }
               return lsts;
          }

          public async Task<string> CheckPPNumberExists(string ppNumber)
          {
               var pp = await _context.Candidates.Where(x => x.PpNo.ToLower() == ppNumber.ToLower()).Select(x => new {x.ApplicationNo, x.FullName}).FirstOrDefaultAsync();
               if (pp==null) return null;
               return pp.ApplicationNo + " - " + pp.FullName;
          }

          public async Task<string> GetCategoryNameFromCategoryId(int id)
          {
               return await _context.Categories.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
          }

          public async Task<string> GetCustomerNameFromCustomerId(int id)
          {
               return await _context.Customers.Where(x => x.Id == id).Select(x => x.CustomerName).FirstOrDefaultAsync();
          }

		public async Task<bool> AddUserAttachments(ICollection<UserAttachment> userattachments)
		{
			foreach(var at in userattachments)
               {
                    _unitOfWork.Repository<UserAttachment>().Add(at);
               }

               return await _unitOfWork.Complete() > 0;
		}
     
          public async Task<string> GetFileUrl(int attachmentid) {
               var attachment = await _context.UserAttachments.FindAsync(attachmentid);
               if (attachment==null) return "";

               var FileName=attachment.url;
               if(string.IsNullOrEmpty(FileName)) {
                    FileName = Directory.GetCurrentDirectory() + "\\assets\\images\\" + attachment.FileName;     //api is the current driectory
               }

               if(FileName.Contains("\\")) FileName = FileName.Replace(@"\\", @"\");

               if(!System.IO.File.Exists(@FileName)) return "";

               //var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

               return FileName;
          }

          public async Task<Pagination<UserDto>> GetAppUsersPaginated(UserParams userParams)
          {
               var qry = _userManager.Users
                    .Where(x => x.UserType== userParams.UserType)
                    .Include(r => r.UserRoles)
                    .ThenInclude(r => r.Role)
                    .OrderBy(u => u.UserName)
                    .Select(u => new UserDto
                    {
                         loggedInEmployeeId=u.loggedInEmployeeId,
                         Username = u.UserName,
                         DisplayName = u.DisplayName,
                         Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                    })
                .AsQueryable();

               if(!string.IsNullOrEmpty(userParams.DisplayName)) qry = qry.Where(x => x.DisplayName==userParams.DisplayName);
               if(!string.IsNullOrEmpty(userParams.Username)) qry = qry.Where(x => x.Username==userParams.Username);
               var TotalCount = await qry.CountAsync();
               if (TotalCount==0) return null;
               
               var take = userParams.PageSize;
               var skip= (userParams.PageIndex-1) * userParams.PageSize;
               
               var temp = await qry.Take(take).Skip(skip).ToListAsync();
               //var users = await qry.Take(userParams.PageSize).Skip((userParams.PageIndex-1)*userParams.PageSize).ToListAsync();
               var users = await qry.Skip((userParams.PageIndex-1)*userParams.PageSize).Take(userParams.PageSize) .ToListAsync();
               return new Pagination<UserDto>(userParams.PageIndex, userParams.PageSize, TotalCount, users);
          }
     }
}

