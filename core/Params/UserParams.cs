using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{
    public class UserParams: ParamPages
    {
        public string UserType {get; set;}
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}