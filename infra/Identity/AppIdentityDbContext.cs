using core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace infra.Identity
{
     public class AppIdentityDbContext : IdentityDbContext<AppUser>
     {
          public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
          {
          }

          protected override void OnModelCreating(ModelBuilder builder)
          {
               base.OnModelCreating(builder);
               //services.AddDefaultIdentity<ApplicationUser().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
               
          }
     }
}