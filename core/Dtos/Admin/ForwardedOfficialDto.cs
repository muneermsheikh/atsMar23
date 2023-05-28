using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos.Admin
{
    public class ForwardedOfficialDto
    {
        public int ForwardCategoryId { get; set; }
        //public int OrderItemId {get; set;}
        //public int CustomerOfficialId { get; set; }
        public string OfficialName {get; set;}
        public string AgentName {get; set;}
        public DateTime DateTimeForwarded { get; set; }=DateTime.Now;
        public DateTime DateOnlyForwarded {get; set;}=DateTime.Now.Date;
        public string EmailIdForwardedTo { get; set; }
        public string PhoneNoForwardedTo { get; set; }
        public string WhatsAppNoForwardedTo { get; set; }
        public int LoggedInEmployeeId {get; set;}

    }
}