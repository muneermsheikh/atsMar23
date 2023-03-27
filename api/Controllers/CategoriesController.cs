

using core.Entities.MasterEntities;
using infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class CategoriesController: ControllerBase
    {
        private readonly ATSContext _context;
        public CategoriesController(ATSContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var products = await _context.Categories.ToListAsync();
            return products;
        }
    }
}
