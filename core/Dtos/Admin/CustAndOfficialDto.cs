using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos.Admin
{
    public class CustAndOfficialDto
    {
        public int CustomerId { get; set; }
        public int OfficialId {get; set;}
        public string AppUserId {get; set;}
        public string CustomerName { get; set; }
        public string City {get; set;}
        public string Country {get; set;}
        public string OfficialTitle {get; set;}
        public string OfficialName { get; set; }
        public string OfficialEmail { get; set; }
        public string OfficialDesignation { get; set; } 
        
    }
}