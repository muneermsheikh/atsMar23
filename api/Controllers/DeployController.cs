using api.Errors;
using api.Extensions;
using core.Dtos;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Process;
using core.Interfaces;
using core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize]     //(Policy = "ProcessEmployeeRole")]
     public class DeployController : BaseApiController
     {
          private readonly IDeployService _deployService;
          private readonly IUnitOfWork _unitOfWork;
          private readonly IEmployeeService _empService;
          private readonly UserManager<AppUser> _userManager;
          public DeployController(IDeployService deployService, IUnitOfWork unitOfWork, 
                UserManager<AppUser> userManager, IEmployeeService empService)
          {
               _userManager = userManager;
               _empService = empService;
               _unitOfWork = unitOfWork;
               _deployService = deployService;
          }

          [Authorize]    //(Roles = "Admin, DocumentControllerAdmin, DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpGet("pending")]
          public async Task<ActionResult<Pagination<DeploymentPendingDto>>> GetPendingDeployments([FromQuery]DeployParams depParam)
          {
     
               var ret = await _deployService.GetPendingDeployments(depParam);
               
               return Ok(ret);
          }


          [Authorize]  //(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpPost("posts")]
          public async Task<ActionResult<DeploymentDtoWithErrorDto>> AddDeploymentTransactions(ICollection<Deploy> deployPosts)
          {
               var loggedInDto = await GetLoggedInUserDto();
               if(deployPosts.Count==0) return BadRequest(new ApiResponse(402, "No input provided"));
               foreach(var dto in deployPosts)
               {
                    if(dto.CVRefId == 0 || dto.Sequence==0 ) return BadRequest(new ApiResponse(402, "Deploy Id or Status not provided"));
                    if(dto.TransactionDate.Year < 2000) dto.TransactionDate = DateTime.Now;
               }

               var returndto = await _deployService.AddDeploymentTransactions(deployPosts, loggedInDto.LoggedInEmployeeId);

               if(returndto.ErrorStrings.Count > 0) {
                    return BadRequest(new ApiResponse(401, "Failed to register the deployment transaction" + 
                         string.Join(Environment.NewLine, returndto.ErrorStrings)));
               }
               
               return Ok(true);
          }

          [HttpGet("deploys/{CVRefId}")]
          public async Task<ActionResult<ICollection<DeploymentDto>>> GetCVRefIdDeployments(int CVRefId)
          {
               var dtos = await _deployService.GetDeployments(CVRefId);
               if (dtos == null) return NotFound(new ApiResponse(402, "No data found"));

               return Ok(dtos);
          }

          [Authorize]  //(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpPut("editSingleTransaction")]
          public async Task<ActionResult<bool>> EditDeploymentTransaction([FromQuery]DeploymentDto deploy)
          {
               return await _deployService.EditDeploymentTransaction(deploy);
          }

          [Authorize]  //(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditDeploymentTransactions( CVReferredDto cvref)
          {
               return await _deployService.EditDeploymentTransactions(cvref);
          }

          [Authorize]  //(Roles = "DocumentControllerProcess, EmigrationExecutive, MedicalExecutive, MedicalExecutiveGAMMCA, ProcessExecutive, VisaExecutiveDubai, VisaExecutiveKSA, VisaExecutiveQatar, VisaExecutiveBahrain")]          
          [HttpDelete("deletetransaction/{deployid}")]
          public async Task<ActionResult<bool>> DeleteDeploymentTransactions(int deployid)
          {
               return await _deployService.DeleteDeploymentTransactions(deployid);
          }

          private async Task<LoggedInUserDto> GetLoggedInUserDto()
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               if (loggedInUser == null) return null;

               var empId = await _empService.GetEmployeeIdFromAppUserIdAsync(loggedInUser.Id);
               var loggedInUserDto = new LoggedInUserDto
               {
                    LoggedIAppUsername = loggedInUser.UserName,
                    LoggedInAppUserEmail = loggedInUser.Email,
                    LoggedInAppUserId = loggedInUser.Id,
                    LoggedInEmployeeId = empId,
                    HasAdminPrivilege = User.IsInRole("Admin")
               };
               return loggedInUserDto;
          }

          [Authorize]
          [HttpGet("depStatus")]
          public async Task<ActionResult<ICollection<DeployStage>>> GetDeploymentStatus()
          {
               var st = await _deployService.GetDeployStatuses();
               if(st==null) return NotFound(new ApiResponse(404, "No records found"));
               return Ok(st);
          }
     
          [HttpGet("cvreferreddto/{cvrefid}")]
          public async Task<ActionResult<CVReferredDto>> GetCVRefDto(int cvrefid)
          {
               var dto = await _deployService.GetDeploymentDto(cvrefid);
               if (dto == null) return NotFound(new ApiResponse(404, "Record not found"));
               return Ok(dto);
          }
     }
}