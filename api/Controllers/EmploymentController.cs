using core.Entities.HR;
using core.Interfaces;
using core.Params;
using core.Dtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class EmploymentController : BaseApiController
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IEmploymentService _employmentService;
          public EmploymentController(IUnitOfWork unitOfWork, IEmploymentService employmentService)
          {
               _employmentService = employmentService;
               _unitOfWork = unitOfWork;
          }

          [Authorize]   //(Roles = "Admn, HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin, DocumentControllerProcess")]
          [HttpGet]
          public async Task<ActionResult<Pagination<Employment>>> GetEmployments([FromQuery] EmploymentParams employmentParams)
          {
              var emps = await _employmentService.GetEmployments(employmentParams);
              
              if(emps == null) return BadRequest("failed to return matching records");

              return Ok(emps);
          }

          [Authorize] //Roles = "Admn, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpPost]
          public async Task<ActionResult<Employment>> AddEmployment(Employment employment)
          {
              // todo - verify object
              var sel = await _unitOfWork.Repository<SelectionDecision>().GetByIdAsync(employment.SelectionDecisionId);
              if (sel == null) return null;

                var emp = await _employmentService.AddEmployment(employment);

                if(emp == null) return BadRequest("failed to add the employment data");

              return Ok(emp);
          }

        /*[Authorize]
        [HttpGet("employment/{cvrefid}")]
        public async Task<Employment> GetEmployment (int cvrefid)
        {
            
            return await _employmentService.GetEmployment(cvrefid);
        }

        [Authorize]
        [HttpGet("employmentsbyorderno/{orderno}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromOrderNo(int orderno)
        {
            return await _employmentService.GetEmploymentDtoFromOrderNo(orderno);
        }
        
        [Authorize]
        [HttpGet("employmentsbydates/{datefrom}/{dateupto}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromOrderNo(DateTime dateform, DateTime dateupto)
        {
            return await _employmentService.GetEmploymentDtoBetwenDates(dateform, dateupto);
        }

        [Authorize]
        [HttpGet("employmentsbycvref/{cvref}")]
        public async Task<ICollection<EmploymentDto>> GetEmploymentsFromCVRef(int cvref)
        {
            return await _employmentService.GetEmploymentDtoFromCVRefId(cvref);
        }


        [Authorize]
        [HttpGet("employmentfromselid/{id}")]
        public async Task<Employment> GetEmploymentFromSelId (int id)
        {
            return await _employmentService.GetEmploymentFromSelId(id);
        }
        */

        [Authorize] //Roles = "Admn, HRManager, HRSupervisor, DocumentControllerAdmin")]
        [HttpPut("employment")]
        public async Task<bool> UpdateEmployment(Employment employment)
        {
            return await _employmentService.EditEmployment(employment);
        }

        [Authorize] //Roles = "Admn, HRManager, HRSupervisor, DocumentControllerAdmin")]
        [HttpDelete("{employmentid}")]
        public async Task<bool> UpdateEmployment(int employmentid)
        {
            return await _employmentService.DeleteEmployment(employmentid);
        }


     }
}