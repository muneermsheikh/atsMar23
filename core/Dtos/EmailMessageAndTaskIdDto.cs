using core.Entities.EmailandSMS;

namespace core.Dtos
{
    public class EmailMessageAndTaskIdDto
    {
        public EmailMessage EmailMessage { get; set; }
        public int TaskId { get; set; }
    }
}