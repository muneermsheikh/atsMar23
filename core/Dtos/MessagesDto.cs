using core.Entities.EmailandSMS;

namespace core.Dtos
{
     public class MessagesDto
    {
          public MessagesDto()
          {
          }

            public ICollection<EmailMessage> emailMessages { get; set; }
            public string ErrorString {get; set;}
            public ICollection<int> CvRefIds {get; set;}
    }
}