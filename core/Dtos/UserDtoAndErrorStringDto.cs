using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Users;

namespace core.Dtos
{
    public class CandidateIdAndErrorStringDto
    {
        public int CandidateId { get; set; }
        public int ApplicationNo {get; set;}
        public string ErrorString { get; set; }
        public ICollection<UserAttachment> UserAttachments {get; set;}
    }
}