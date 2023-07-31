using core.Dtos;
using core.Dtos.Admin;
using core.Entities.EmailandSMS;
using core.Entities.Orders;

namespace core.Interfaces
{
     public interface IComposeMessagesForAdmin
    {
         //Admin
        Task<EmailMessage> AckEnquiryToCustomer(Order order);
        Task<EmailMessage> ForwardEnquiryToHRDept(Order order);
        ICollection<EmailMessage> ComposeCVFwdMessagesToClient(ICollection<CVFwdMsgDto> fwdMsgsDto, LoggedInUserDto LoggedInDto);
        EmailMessage ComposeSelDecisionRemindersToClient(CustAndOfficialDto custAndOfficialDto, ICollection<CVReferredDto> cvreferredDto, LoggedInUserDto loggeddto);
        Task<List<EmailMessage>> AdviseSelectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> selections, 
            string senderuserName, DateTime datenow, string senderEmailAddress, int senderEmployeeId);
        Task<SMSMessage> AdviseSelectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selection);
        List<EmailMessage> AdviseRejectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> rejectionsDto,  // ICollection<RejDecisionToAddDto> rejectionsDto, 
            string loggedInUserName, DateTime dateTimeNow, string SenderEmailAddresss, int loggedInEmployeeId);
        Task<SMSMessage> AdviseRejectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selection);
    }
}