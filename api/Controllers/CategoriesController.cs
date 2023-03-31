using api.Errors;
using core.Entities;
using core.Entities.MasterEntities;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [ApiController]
    [Route("api/controller")]
    public class CategoriesController: ControllerBase
    {
          
          private readonly IGenericRepository<Category> _categoryRepo;
          private readonly IGenericRepository<Customer> _customerRepo;

        public CategoriesController(IGenericRepository<Category> categoryRepo, IGenericRepository<Customer> customerRepo)
        {
               _customerRepo = customerRepo;
               _categoryRepo = categoryRepo;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var products = await _categoryRepo.ListAllAsync();
            if (products == null) return BadRequest(new ApiResponse(404, "The Categories requested was not found"));
            return Ok(products);
        }

        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var product = await _categoryRepo.GetByIdAsync(id);
            if (product == null) return BadRequest(new ApiResponse(404, "The Category requested was not found"));
            return Ok(product);
        }
    }
}
