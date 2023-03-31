using System;

namespace core.Entities.EmailandSMS
{
    public class SMSMessage: BaseEntity
    {
        public SMSMessage()
        {
        }

        public SMSMessage(int userId,  string phoneNo, DateTime composedOn, string sMSText)
        {
            UserId = userId;
            PhoneNo = phoneNo;
            SMSText = sMSText;
            ComposedOn = composedOn;
        }

        public int SequenceNo { get; set; }
        public int UserId { get; set; }
        public DateTime ComposedOn {get; set;}
        public DateTime SMSDateTime { get; set; }
        public string PhoneNo { get; set; }
        public string SMSText { get; set; }
        public string DeliveryResult { get; set; }
    }
}