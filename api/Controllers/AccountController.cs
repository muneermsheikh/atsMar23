using api.DTOs;
using api.Errors;
using api.Extensions;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using core.Params;
using infra.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Headers;
using core.Entities.AccountsNFinance;

namespace api.Controllers
{
     public class AccountController : BaseApiController
     {
          private readonly UserManager<AppUser> _userManager;
          private readonly SignInManager<AppUser> _signInManager;
          private readonly ITokenService _tokenService;
          private readonly IMapper _mapper;
          private readonly IUserService _userService;
          private readonly AppIdentityDbContext _identityContext;
          private readonly ITaskService _taskService;
          private readonly ITaskControlledService _taskControlledService;
          private readonly IEmployeeService _empService;
          private readonly IConfiguration _config;
          //private readonly RoleManager<AppRole> _roleManager;
          
          public AccountController(
               UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
               ITokenService tokenService, 
               //RoleManager<AppRole> roleManager, 
               ITaskControlledService taskControlledService,
               IMapper mapper, IUserService userService, AppIdentityDbContext identityContext,
               ITaskService taskService, IEmployeeService empService, IConfiguration config
               )
          {
               _userService = userService ?? throw new ArgumentNullException(nameof(userService));
               _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
               _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
               _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
               _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
               _identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
               _taskControlledService=taskControlledService;
               _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
               _empService = empService ?? throw new ArgumentNullException(nameof(empService));
               _config = config ?? throw new ArgumentNullException(nameof(config));
               //_roleManager = roleManager;
          }


          //[Authorize]
          [HttpGet]
          public async Task<ActionResult<UserDto>> GetCurrentUser()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser==null) return BadRequest("User email not found");
               return new UserDto
               {
                    loggedInEmployeeId = loggedInUser.loggedInEmployeeId,
                    Email = loggedInUser.Email,
                    Token = await _tokenService.CreateToken(loggedInUser),
                    DisplayName = loggedInUser.DisplayName 
               };  
          }
          

          [HttpGet("emailexists")]
          public async Task<AppUser> AppUserOfEmail([FromQuery] string email)
          {
               var appuser = await _userManager.FindByEmailAsync(email);
               if (appuser == null) return null;
               return appuser;

          }
          
          [HttpGet("ppexists")]
          public async Task<ActionResult<string>> CheckPPNumberExistsAsync([FromQuery] string ppnumber)
          {
               var pp = await _userService.CheckPPNumberExists(ppnumber);
               return pp;
          }
          
          
          [HttpGet("aadahrexists/{aadharno}")]
          public async Task<ActionResult<bool>> CheckAadharNoExistsAsync([FromQuery] string aadharno)
          {
               return await _userService.CheckAadharNoExists(aadharno);
          }

          
          [HttpPost("login")]
          public async Task<ActionResult<core.Dtos.UserDto>> Login(LoginDto loginDto)
          {
               //exclude user.roles till role management is implemented
               /*var user = await _userManager.Users.Where(x => x.Email == loginDto.Email)
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    //.Select(x => new {x.Id, x.Gender, x.DisplayName, x.Email, x.loggedInEmployeeId})
                    .FirstOrDefaultAsync();
               */
               var user = await _userManager.Users.Where(x => x.Email == loginDto.Email)
                    //.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    //.Select(x => new {x.Id, x.Gender, x.DisplayName, x.Email, x.loggedInEmployeeId})
                    .FirstOrDefaultAsync();
               
               if (user == null) return Unauthorized(new ApiResponse(401));
               
               var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
               if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
          
               //authorization
               var claims = new List<Claim>();
               claims.Add(new Claim("username", user.UserName));
               claims.Add(new Claim("displayname", user.KnownAs));
               
               // Add roles as multiple claims
               if (user.UserRoles != null) {
                    foreach(var role in user.UserRoles) 
                    {
                         //claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                    }
               }
               
               // Optionally add other app specific claims as needed
               //claims.Add(new Claim("UserState", UserState.ToString()));
               var loggedInEmployeeId=user.loggedInEmployeeId;
               if(user.loggedInEmployeeId == 0) {
                    loggedInEmployeeId = await _empService.GetEmployeeIdFromAppUserIdAsync(user.Id);
               }
               
               var userdto = new core.Dtos.UserDto
               {
                    loggedInEmployeeId = loggedInEmployeeId,
                    Username = user.UserName,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Token = await _tokenService.CreateToken(user),
               };

               return userdto;
          }

          [Authorize]
          [HttpGet("users")]
          public async Task<ActionResult<ICollection<UserDto>>> GetUsers (AppUserSpecParams userParams)
          {
               var users = await _userManager.Users.ToListAsync();
               return  Ok(_mapper.Map<ICollection<UserDto>>(users));
          }
          
          [HttpPost("RegisterNewCandidate"), DisableRequestSizeLimit]
          public async Task<ActionResult<ApiReturnDto>> Upload()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               var returnDto = new ApiReturnDto();

               var userattachmentlist = new List<UserAttachment>();
               
               try
               {
                    var modelData = JsonSerializer.Deserialize<RegisterDto>(Request.Form["data"],   //THE CNDIDATE OBJECT
                         new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    
                    var files = Request.Form.Files;
                    //create the candidate object frist, because the file attachment to save later needs candidate.Id
                    var CandidateDtoWithErr = await CreateAppUserAndCandidate(modelData, loggedInUser==null ? 0 : loggedInUser.loggedInEmployeeId);
                    if(!string.IsNullOrEmpty(CandidateDtoWithErr.ErrorString)) {
                         //returnDto.ErrorMessage=CandidateDtoWithErr.ErrorString;
                         return BadRequest(new ApiResponse(400, CandidateDtoWithErr.ErrorString));
                         //return returnDto;
                    }
                    var candidateCreated = CandidateDtoWithErr.CandidateId;
                    var ApplicationNoString = CandidateDtoWithErr.ApplicationNo.ToString().Trim();
                    //candidate is created, now check for any file attachment to download and save.
                    //data is the key that is being passed from client side
                    //file in params will have the posted file
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    var memoryStream = new MemoryStream();

                    foreach (var file in files)
                    {
                         if (file.Length==0) continue;
                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         
                         var fullPath = Path.Combine(pathToSave, ApplicationNoString + "-" + fileName);        //physical path
                         if(System.IO.File.Exists(fullPath)) continue;
                         var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }
                         
                         if(fileName.Contains("imageFile")) {
                              await _userService.UpdateCandidatePhoto(CandidateDtoWithErr.CandidateId, @"./Assets/Images/" + ApplicationNoString + "-" + fileName);
                              var attachments = new List<UserAttachment>();
                              attachments.Add(new UserAttachment{
                                   CandidateId = CandidateDtoWithErr.CandidateId,
                                   AttachmentType="Photograph",
                                   FileName=ApplicationNoString + "-" + fileName,
                                   url = @"./Assets/Images/" + ApplicationNoString + "-" + fileName,
                                   DateUploaded=DateTime.Now,
                                   UploadedByEmployeeId=loggedInUser==null ? 0: loggedInUser.loggedInEmployeeId
                              });

                              await _userService.AddUserAttachments(attachments);
                         }
                    }
                    
                    
                    returnDto.ReturnInt=CandidateDtoWithErr.ApplicationNo;
                    return Ok(returnDto);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
               
          }

          //registers individuals. For customers and vendors, it will register the users for customers that exist
          //the IFormFile collection has following prefixes to filenames:
          //pp: passport; ph: photo, ec: educational certificates, qc: qualification certificates
          
          private async Task<CandidateIdAndErrorStringDto> CreateAppUserAndCandidate(RegisterDto registerDto, int loggedInEmployeeId) 
          {
               var dtoToReturn = new CandidateIdAndErrorStringDto();

               //attempt to create AppUser
               var existingAppUser = await AppUserOfEmail(registerDto.Email);
               if (existingAppUser != null) {
                    dtoToReturn.ErrorString = "Email address is in use";
                    return dtoToReturn; }

               if (!string.IsNullOrEmpty(registerDto.AadharNo) &&  await _userService.CheckCandidateAadharNoExists(registerDto.AadharNo)) {
                    dtoToReturn.ErrorString = "Aadhar Number is in use";
                    return dtoToReturn; }

               if (!string.IsNullOrEmpty(registerDto.PpNo) && !string.IsNullOrEmpty(await _userService.CheckPPNumberExists(registerDto.PpNo))) {
                    dtoToReturn.ErrorString = "Passport Number is in use";
                    return dtoToReturn; }

               if (registerDto.UserPhones != null && registerDto.UserPhones.Count() > 0)
               {
                    foreach (var ph in registerDto.UserPhones) {
                         if (string.IsNullOrEmpty(ph.MobileNo)) {
                              dtoToReturn.ErrorString = "Mobile Number cannot be blank";
                              return dtoToReturn; } }
               }
               
               if(string.IsNullOrEmpty(registerDto.Password)) {
                    dtoToReturn.ErrorString = "Password not provided";
                    return dtoToReturn;
               }

               //create and save AppUser 
               var user = new AppUser();

               user = new AppUser
               {
                    UserType = registerDto.UserType,
                    DisplayName = registerDto.KnownAs, // registerDto.DisplayName,
                    KnownAs = registerDto.KnownAs,
                    Gender = registerDto.Gender,
                    PhoneNumber = registerDto.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault(),
                    Email = registerDto.Email,
                    UserName = registerDto.Email
               };
               var result = await _userManager.CreateAsync(user, registerDto.Password);
               if (!result.Succeeded) {
                    dtoToReturn.ErrorString = result.Errors.Select(x => x.Description).FirstOrDefault();
                    return dtoToReturn;
               }

               // if(string.IsNullOrEmpty(registerDto.UserRole)) registerDto.UserRole="Candidate";
               //if(!await _roleManager.RoleExistsAsync("Candidate")) {
                    //var succeeded = await _roleManager.CreateAsync(new AppRole{Name="Candidate"}); }    
               //
               //   **role** 
               //var roleResult = await _userManager.AddToRoleAsync(user, registerDto.UserRole);
               //if (!roleResult.Succeeded) {
                    //dtoToReturn.ErrorString = roleResult.Errors.ToString();
                    //return dtoToReturn; }
               //
               registerDto.DisplayName = registerDto.DisplayName ?? user.DisplayName;
               registerDto.PlaceOfBirth = registerDto.PlaceOfBirth ?? "";
               
               var userDtoToReturn = new UserDto
               {
                    DisplayName = user.DisplayName,
                    Token = await _tokenService.CreateToken(user),
                    Email = user.Email
               };

               registerDto.AppUserId = user.Id;   //   **TODO** check of any existing number in UserPhones 

               // create candidate entity
               var candidateCreated = await _userService.CreateCandidateAsync(registerDto, loggedInEmployeeId);
               if(candidateCreated == null) {
                    //failed, delete appuser created
                    await _userManager.DeleteAsync(user);
                    dtoToReturn.ErrorString="Failed To Create the candidate";
                    return dtoToReturn;
               } else {
                    //user.loggedInEmployeeId=candidateCreated.Id;
                    user.loggedInEmployeeId=loggedInEmployeeId;
                    userDtoToReturn.ObjectId=candidateCreated.Id;
                    
                    await _userManager.UpdateAsync(user);
               }
     
          
               dtoToReturn.CandidateId = candidateCreated.Id;
               dtoToReturn.UserAttachments = candidateCreated.UserAttachments;
               dtoToReturn.ApplicationNo = candidateCreated.ApplicationNo;
               return  dtoToReturn;
          }
          

          [Authorize]    //(Roles ="Admin, HRManager, HRSupervisor")]
          [HttpPost("registeremployee")]
          public async Task<ActionResult<core.Dtos.UserDto>> RegisterEmployee(RegisterEmployeeDto registerDto )
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               if(string.IsNullOrEmpty(registerDto.Email) 
                    || string.IsNullOrEmpty(registerDto.AadharNo)
                    || string.IsNullOrEmpty(registerDto.Department)) 
               return BadRequest("Email ID, Aadhar Card and Display Name - all are mandatory to register an employee");
               
               //check if user email already on record
               var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
               if(emailExists != null)
                    return new BadRequestObjectResult(new ApiValidationErrorResponse { Errors = new[] { "Email address is in use by Identity Entity" } });

               if (registerDto.EmployeePhones != null && registerDto.EmployeePhones.Count() > 0)
               {
                    foreach (var ph in registerDto.EmployeePhones) {
                         if (string.IsNullOrEmpty(ph.MobileNo)) return BadRequest(new ApiResponse(400, "mobile no cannot be blank"));
                    }
               }

               //create and save AppUser IdentityObject
               var user = new AppUser
               {
                    UserType = "Employee",
                    DisplayName = registerDto.KnownAs, // registerDto.DisplayName,
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                    PhoneNumber = registerDto.EmployeePhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault(),
                    KnownAs = registerDto.DisplayName,
                    Gender = registerDto.Gender
               };
               var result = await _userManager.CreateAsync(user, registerDto.Password);
               if (!result.Succeeded)  return BadRequest(new ApiResponse(404, "Unable to create Identity record - " + result.Errors.Select(X => X.Description).FirstOrDefault()));
          
               if(!string.IsNullOrEmpty(registerDto.UserRole)) {
                    var roleResult = await _userManager.AddToRoleAsync(user, registerDto.UserRole);
                    if (!result.Succeeded) return BadRequest(new ApiResponse(404, "Unable to Add role to the Identity - " + result.Errors.Select(X => X.Description).FirstOrDefault()));
               }
               
               registerDto.DisplayName = registerDto.DisplayName ?? user.DisplayName;
               registerDto.PlaceOfBirth = registerDto.PlaceOfBirth ?? "";
               
               //save the objects in DataContext database
               registerDto.AppUserId = user.Id;
               //*** flg not working..
               /*
               if (registerDto.UserPhones != null)
               {    //ensure no duplicate user phones in the collection
                    var qry = (from p in registerDto.UserPhones
                              group p by p.MobileNo into g
                              where g.Count() > 1
                              select g.Key);
                    if (qry != null) registerDto.UserPhones = null;     //disallow if any duplicate numbers
               }
               */

               var empAdded = await _userService.CreateEmployeeAsync(registerDto);
               if (empAdded == null)  {
                    //failed to create empoyee object
                    //delete the AppUser Object
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new ApiResponse(404, "failed to create employee"));
               }

               user.loggedInEmployeeId=empAdded.Id;
               await _userManager.UpdateAsync(user);

               var userDtoToReturn = new core.Dtos.UserDto
                    {
                         loggedInEmployeeId = empAdded.Id,
                         DisplayName = user.DisplayName,
                         Token = await _tokenService.CreateToken(user),
                         Email = user.Email
                    };
               return userDtoToReturn;
          }

         private async Task<string> WriteFile(IFormFile file)
          {
               //bool isSaveSuccess = false;
               string fileName;
               try
               {
                    var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                    if (!Directory.Exists(pathBuilt))
                    {
                         Directory.CreateDirectory(pathBuilt);
                    }

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                         await file.CopyToAsync(stream);
                    }

                    //isSaveSuccess = true;
               }
               catch (Exception e)
               {
                    return e.Message;
               }

               return "";
          }


          [Authorize]    //(Roles ="Admin, HRManager")]
          [HttpDelete("user/{useremail}")]
          public async Task<ActionResult<bool>> DeleteIdentityUser (string useremail)
          {
               var user = await _userManager.FindByEmailAsync(useremail);
               if (user==null) {
                    return BadRequest(new ApiResponse(400, "no user with the selected email exists"));
               }
               var result = await _userManager.DeleteAsync(user);

               return result.Succeeded;
          }
     
          private async Task<bool> UserExists(string username)
          {
               return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
          }
     }
}