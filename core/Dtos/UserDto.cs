using System.Collections.Generic;

namespace core.Dtos
{
    public class UserDto
    {
        public int loggedInEmployeeId {get; set;}
        public int ObjectId {get; set;}
        public string Username {get; set;}
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public int PageSize {get; set;}
        public int PageIndex {get; set;}
        public ICollection<string> Roles {get; set;}
    }
}