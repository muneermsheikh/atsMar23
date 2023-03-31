using core.Dtos;
using core.Entities.EmailandSMS;
using core.Params;

namespace core.Interfaces
{
     public interface IEmailService
    {
        EmailMessage SendEmail(EmailMessage emailMessage, ICollection<string> attachments);
        Task<EmailMessage> SaveEmailMessage(EmailMessage message);
        Task SendEmailAsync(EmailMessage emailMessage);
        Task<Pagination<EmailMessage>> GetEmailMessageOfLoggedinUser(EmailMessageSpecParams msgParams);

    }
}