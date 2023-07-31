using System.Collections.Generic;
using core.Entities.Admin;
using core.Entities.Orders;

namespace core.Entities
{
    public class CustomerOfficial: BaseEntity
    {
          public CustomerOfficial()
          {
          }

          public CustomerOfficial(int customerId, string gender, string title, string officialName, string designation, 
            string phoneNo, string mobile, string email, string imageUrl, bool login, bool isValid)
          {
               CustomerId = customerId;
               Gender = gender;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               PhoneNo = phoneNo;
               Mobile = mobile;   //used as whatsApp
               Email = email;
               ImageUrl = imageUrl;
               IsValid = isValid;
               LogInCredential = login;
          }

        public CustomerOfficial(string gender, string title, string officialName, string designation, 
            string phoneNo, string mobile, string email, string imageUrl, bool login)
          {
               Gender = gender;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               PhoneNo = phoneNo;
               Mobile = mobile;   //also used as whatsApp
               Email = email;
               ImageUrl = imageUrl;
               LogInCredential = login;
          }

        public bool LogInCredential {get; set;}=false;
        public string AppUserId { get; set; }
        public int CustomerId { get; set; }
        public string Gender { get; set; }="M";
        public string Title { get; set; }
        public string OfficialName { get; set; }
        public string KnownAs {get; set;}
        public string Designation { get; set; }
        public string Divn {get; set;}  
        public string PhoneNo { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public bool IsValid { get; set; }=true;
        //public Customer Customer {get; set;}
        public ICollection<DLForwardToAgent> DLForwardToAgents {get; set;}
    }
}