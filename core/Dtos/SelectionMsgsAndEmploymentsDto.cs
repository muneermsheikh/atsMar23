using System.Collections.Generic;
using core.Entities.EmailandSMS;

namespace core.Dtos
{
     public class SelectionMsgsAndEmploymentsDto
    {
        public ICollection<EmailMessage> EmailMessages {get; set;}
        public ICollection<EmploymentDto> EmploymentDtos {get; set;}
        public ICollection<int> CvRefIdsAffected {get; set;}
    }
}