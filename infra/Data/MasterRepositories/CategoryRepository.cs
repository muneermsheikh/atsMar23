using core.Entities.MasterEntities;
using core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace infra.Data.MasterRepositories
{
     public class CategoryRepository : ICategoryRepository
     {
          private readonly ATSContext _context;
          public CategoryRepository(ATSContext context)
          {
               _context = context;
          }

          public async Task<IReadOnlyList<Category>> GetCategoryList()
          {
               return await _context.Categories.ToListAsync();
          }

          public async Task<Category> GetCategryBIdAsync(int id)
          {
               return await _context.Categories.FindAsync(id);
          }
     }
}