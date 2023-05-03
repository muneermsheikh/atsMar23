using api.Errors;
using AutoMapper;
using core.Dtos;
using core.Entities;
using core.Entities.Identity;
using core.Interfaces;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     //[Authorize(Policy = "Employee, CustomerMaintenanceRole")]
     [Authorize]
     public class CustomersController : BaseApiController
     {
          private readonly IGenericRepository<Customer> _custRepo;
          private readonly IUnitOfWork _unitOfWork;
          private readonly ICustomerService _customerService;
          private readonly UserManager<AppUser> _usermanager;
          public CustomersController(IGenericRepository<Customer> custRepo, IMapper mapper,
            IUnitOfWork unitOfWork, ICustomerService customerService, UserManager<AppUser> usermanager)
          {
               _usermanager = usermanager;
               _customerService = customerService;
               _unitOfWork = unitOfWork;
               _custRepo = custRepo;
          }

        //[Authorize] //(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPost("registercustomers")]
        public async Task<ActionResult<ICollection<CustomerDto>>> RegisterCustomers(ICollection<RegisterCustomerDto> dtos)
        {
            var customers = await _customerService.AddCustomers(dtos);
            if(customers == null) return BadRequest(new ApiResponse(400, "failed to save the customers"));
            return Ok(customers);

        }

        //[Authorize] //(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> RegisterCustomer(RegisterCustomerDto dto)
        {
            return await _customerService.AddCustomer(dto);
        }

        //[Authorize] //(Roles="Admin, DocumentControllerAdmin, HRManage, HRSupervisor")]
        [HttpGet]
        public async Task<ActionResult<Pagination<Customer>>> GetCustomers([FromQuery] CustomerSpecParams custParams)
        {
            if (custParams.CustomerCityName == "All") custParams.CustomerCityName="";
            var specs = new CustomerWithOfficialsSpecs(custParams);
            var countSpec = new CustomersWithFiltersForCountSpecs(custParams);
            var customers = await _unitOfWork.Repository<Customer>().ListAsync(specs);
            var totalCount = await _unitOfWork.Repository<Customer>().CountAsync(countSpec);

            return Ok(new Pagination<Customer>(custParams.PageIndex, custParams.PageSize, totalCount, customers));

        }

        [HttpGet("customersBrief")]
        public async Task<ActionResult<ICollection<CustomerBriefDto>>> GetCustomersBrief([FromQuery]CustomerSpecParams custParams)
        {
            var briefs = await _customerService.GetCustomersBriefAsync(custParams);
            if(briefs==null) return NotFound("No data returned");
            return Ok(briefs);
        }

        //[Authorize]
        [HttpGet("associateidandnames/{usertype}")]
        public async Task<ActionResult<ICollection<CustomerIdAndNameDto>>> GetCustomerIdAndNames(string usertype)
        {
            var dto = await _customerService.GetCustomerIdAndName(usertype);
            if (dto==null) return NotFound(new ApiResponse(401, "nO valid data found"));
            return Ok(dto);
        }

        [HttpGet("clientidandnames")]
        public async Task<ActionResult<ICollection<ClientIdAndNameDto>>> GetClientIdAndNames()
        {
            var dto = await _customerService.GetClientIdAndName();
            if (dto==null) return NotFound(new ApiResponse(401, "nO valid data found"));
            return Ok(dto);
        }


        //[Authorize]
        [HttpGet("associateidamecitycountry/{usertype}")]
        public async Task<ActionResult<ICollection<CustomerIdAndNameDto>>> GetCustomerIdameCityCountry(string usertype)
        {
            var dto = await _customerService.GetCustomerIdAndName(usertype);
            if (dto==null) return NotFound(new ApiResponse(401, "nO valid data found"));
            return Ok(dto);
        }
        //[Authorize]
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int Id)
        {
            var cust = await _customerService.GetCustomerByIdAsync(Id);
            if (cust==null) return NotFound(new ApiResponse(404, "Not Found"));
            return Ok(cust);
        }

        [HttpGet("customerBriefById/{id}")]
        public async Task<ActionResult<CustomerBriefDto>> GetCustomerBriefById(int Id)
        {
            var cust = await _customerService.GetCustomerBriefById(Id);
            if (cust==null) return NotFound(new ApiResponse(404, "Not Found"));
            return Ok(cust);
        }

        [HttpGet("customernamefromId/{id}")]
        public async Task<ActionResult<string>> GetCustomerNameFromId(int Id)
        {
            var cust = await _customerService.GetCustomerNameFromId(Id);
            if (cust==null) return Ok("not on record");
            return Ok(cust);
        }

        //[Authorize(Roles="Admin, DocumentControllerAdmin, HRManager")]
        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            var succeeded = await _customerService.EditCustomer(customer);

            if (succeeded) return Ok(true);

            return BadRequest();
        }

        //[Authorize]
        [HttpGet("officialidandname/{custType}")]
        public async Task<ActionResult<ICollection<CustomerOfficialDto>>> CustomerOfficialIdAndNames(string custType)
        {
            var users = await _customerService.GetCustomerIdAndName(custType);
            return Ok(users);
        }
        
        
        //[Authorize(Roles="Admin, DocumentControllerAdmin, HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin, DocumentControllerProcess")]
        [HttpGet("agentdetails")]
        public async Task<ActionResult<ICollection<CustomerOfficialDto>>> GetCustomerOfficialIds()
        {
            var users = await _customerService.GetOfficialDetails();
            return Ok(users);
        }

        //[Authorize]
        [HttpGet("customerCities/{customerType}")]
        public async Task<ICollection<CustomerCity>> GetCustomerCities (string customerType)
        {
            return await _customerService.GetCustomerCityNames(customerType);
        }
        
        private async Task<ActionResult<bool>> CheckEmailExistsAsync(string email)
        {
            return await _usermanager.FindByEmailAsync(email) != null;
        }
    }
}