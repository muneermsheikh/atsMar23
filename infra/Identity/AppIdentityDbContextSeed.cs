using core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace infra.Identity
{
     public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsyc(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser{
                    DisplayName = "Munir",
                    Email = "munir.sheikh@live.com",
                    UserName = "munir.sheikh@live.com",
                    Address = new Address {
                        FirstName  ="Munir",
                        FamilyName = "Sheikh",
                        City = "Mumbai",
                        Pin = "400018"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}