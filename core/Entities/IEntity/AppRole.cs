using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;

namespace core.Entities.Identity
{
     //[NotMapped]
     public class AppRole  : IdentityRole
    {
        
        public ICollection<AppUserRole> UserRoles {get; set; }
    }
}
