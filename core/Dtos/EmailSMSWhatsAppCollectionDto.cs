using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;

namespace core.Dtos
{
    public class EmailSMSWhatsAppCollectionDto
    {
        public ICollection<EmailMessage> EmailMessages { get; set; }
        public ICollection<SMSMessage> SMSMessages { get; set; }
        public ICollection<SMSMessage> WhatsAppMessages { get; set; }
        
    }
}