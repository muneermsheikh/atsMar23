using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core.Dtos
{
    public class UserHistoryDto
    {
        public UserHistoryDto()
        {
        }

        public UserHistoryDto(string personName, int personId, string emailId, int applicationNo)
        {
            Name = personName;
            PersonId = personId;
            EmailId = emailId;
            ApplicationNo = applicationNo;
        }

        public UserHistoryDto(int id, string partyName, int personId, string emailId, int applicationNo
            //, ICollection<UserHistoryItemDto> historyitems
            )
        {
            Id = id;
            Name = partyName;
            PersonId = personId;
            EmailId = emailId;
            ApplicationNo = applicationNo;
            //UserHistoryItems = historyitems;
        }

        public int Id {get; set;}
        public string Gender {get; set;}
        public int? UserHistoryHeaderId {get; set;}
        public bool? Checked { get; set; }
        public string Source {get; set;}
        public string CategoryRef { get; set; }
        public string CategoryName { get; set; }
        public string ResumeId { get; set; }
        public string Name {get; set;}
        //public string PersonType {get; set;}
        public int? PersonId {get; set;}
        public string PersonType { get; set; }
        public string EmailId {get; set;}
        public string AlternateEmailId { get; set; }
        public string MobileNo { get; set; }
        public string AlternatePhoneNo { get; set; }
        public int? ApplicationNo {get; set;}
        public DateTime? CreatedOn {get; set;}
        public bool? Concluded { get; set; }
        public string ConcludedByName {get; set;}
        public string Status { get; set; }
        public string UserName { get; set; }
        public ICollection<UserHistoryItemDto> UserHistoryItems { get; set; }
    }
}