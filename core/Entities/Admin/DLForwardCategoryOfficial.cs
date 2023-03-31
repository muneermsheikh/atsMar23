using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class DLForwardCategoryOfficial: BaseEntity
    {
        public DLForwardCategoryOfficial()
		{
		}

        public DLForwardCategoryOfficial(int customerofficialid, DateTime dateforwarded, string emailid, string phoneno, string whatsappno, int loggedinempid)
        {
            CustomerOfficialId = customerofficialid;
            DateTimeForwarded = dateforwarded;
            DateOnlyForwarded = dateforwarded.Date;
            EmailIdForwardedTo = emailid;
            PhoneNoForwardedTo = phoneno;
            WhatsAppNoForwardedTo = whatsappno;
            LoggedInEmployeeId = loggedinempid;
        }

		public int DLForwardCategoryId { get; set; }
        public int OrderItemId {get; set;}
        public int CustomerOfficialId { get; set; }
        public string AgentName {get; set;}
        public DateTime DateTimeForwarded { get; set; }=DateTime.Now;
        public DateTime DateOnlyForwarded {get; set;}=DateTime.Now.Date;
        public string EmailIdForwardedTo { get; set; }
        public string PhoneNoForwardedTo { get; set; }
        public string WhatsAppNoForwardedTo { get; set; }
        public int LoggedInEmployeeId {get; set;}

    }
}