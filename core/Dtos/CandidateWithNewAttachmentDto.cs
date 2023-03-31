using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Users;

namespace core.Dtos
{
    public class CandidateWithNewAttachmentDto
    {
        public Candidate Candidate {get; set;}
        public ICollection<UserAttachment> NewAttachments{get; set;}
    }
}