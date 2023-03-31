using core.Entities.EmailandSMS;

namespace core.Dtos
{
    public class EmailMessageDto
    {
        public EmailMessage EmailMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}