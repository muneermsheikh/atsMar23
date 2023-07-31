using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.Entities.Admin
{
    public class UserHistory: BaseEntity
    {
        public UserHistory()
        {
        }

        public UserHistory( string personname, ICollection<UserHistoryItem> items)
        {
            //PersonId = personid;
            Name = personname;
            UserHistoryItems = items;
        }
        public UserHistory(string personname)
        {
            //PersonId = personid;
            Name = personname;
        }
        public string Gender { get; set; }
        public string Age {get; set;}
        public int? UserHistoryHeaderId { get; set; }
        public bool? Checked { get; set; }
        public string Source { get; set; }
        public string CategoryRef { get; set; }
        public string ResumeId { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string CurrentLocation { get; set; }
        public string City { get; set; }
        public string Name {get; set;}
        public string PersonType {get; set;}
        public int? PersonId {get; set;}
        public string EmailId { get; set; }
        public string AlternateEmailId { get; set; }
        public string MobileNo {get; set;}
        public string AlternatePhoneNo { get; set; }
        public string Education { get; set; }
        public string CTC { get; set; }
        [MaxLength(25)]
        public string WorkExperience { get; set; }
        

        public int? ApplicationNo {get; set;}
        public DateTime CreatedOn {get; set;}
        public bool? Concluded { get; set; }
        public string Status {get; set;}
        public DateTime? StatusDate { get; set; }
        public int? StatusByUserId { get; set; }
        public string UserName { get; set; }
        public DateTime? ConcludedOn {get; set;}
        public int? ConcludedById { get; set; }
        public ICollection<UserHistoryItem> UserHistoryItems {get; set;}
    }
}