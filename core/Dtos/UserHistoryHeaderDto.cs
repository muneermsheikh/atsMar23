using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class UserHistoryHeaderDto
    {
        public int Id { get; set; }
        public string CategoryRefCode { get; set; }
        public string CategoryRefName { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignedOn { get; set; }
        public DateTime CompleteBy { get; set; }
        public string Status { get; set; }
        public bool Concluded {get; set;}
        public int totalCount { get; set; }
        public int TotalNotContactible { get; set; }
        public int TotalContacted { get; set; }
        public int TotalPositive { get; set; }
        public int TotalNegative { get; set; }
    }
}