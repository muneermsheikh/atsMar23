using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Admin
{
    public class UserHistoryItem: BaseEntity
    {
        public UserHistoryItem()
        {
        }

        public UserHistoryItem(string inout, int userHistoryId, string phoneno, DateTime? dateOfContact, 
            int loggedInUserId, string subject, string categoryref, int contactResult,
            string personname, string contactResultName, bool composeMessage, string gistOfDisc)
        {
            IncomingOutgoing = inout;
            UserHistoryId = userHistoryId;
            PhoneNo = phoneno;
            DateOfContact = Convert.ToDateTime(dateOfContact);
            LoggedInUserId = loggedInUserId;
            Subject = subject;
            CategoryRef = categoryref;
            ContactResultId = contactResult;
            ComposeEmailMessage = composeMessage;
            GistOfDiscussions = gistOfDisc;
            ContactResultName = contactResultName;
            PersonName = personname;
        }

        public string IncomingOutgoing {get; set;}
        public int UserHistoryId {get; set;}
        public string PhoneNo {get; set;} 
        public string Subject {get; set;}
        public string CategoryRef {get; set;}
        public int PersonId {get; set;}
        public string PersonType {get; set;}
        public string PersonName {get; set;}
        [Required]
        public DateTime DateOfContact { get; set; }=DateTime.Now;
        [Required]
        public int LoggedInUserId { get; set; }
        public string LoggedInUserName {get; set;}
        [Required]
        public int ContactResultId { get; set; }
        public string ContactResultName {get; set;}
        public string GistOfDiscussions { get; set; }
        [NotMapped]
        public bool ComposeEmailMessage {get; set;}
        public bool ComposeSMS {get; set;}

    }
}