using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class AssessmentsOfACandidateIdDto
    {
        public string ChecklistedByName {get; set;}
        public DateTime ChecklistedOn {get; set;}
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public string CategoryRef {get; set;}
        public DateTime? AssessedOn { get; set; }
        public string AssessedByName {get; set;}
        
    }
}