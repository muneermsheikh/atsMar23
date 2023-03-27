
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;

namespace infra.Data
{
     public class ATSContext : DbContext
     {
          public ATSContext(DbContextOptions options) : base(options)
          {
          }

          public DbSet<Category> Categories {get; set;}
     }
}