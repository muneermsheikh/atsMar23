using core.Dtos;
using core.Entities.Admin;
using core.Entities.EmailandSMS;

namespace core.Interfaces
{
     public interface IComposeMessagesForHR
    {
          //HR
        
        
        Task<ICollection<EmailMessage>> ComposeMessagesToDocControllerAdminToForwardCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn);
        Task<EmailMessage> ComposeHTMLToAckToCandidateByEmail(CandidateMessageParamDto candidate);
        Task<SMSMessage> ComposeMsgToAckToCandidateBySMS(CandidateMessageParamDto candidate);
        Task<EmailMessage> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId);
        Task<EmailSMSWhatsAppCollectionDto> ComposeMsgsToForwardOrdersToAgents(DLForwardToAgent dlforward, int LoggedInEmpId, string LoggedInEmpKNownas, string LoggedInEmpEmail);
        EmailMessage ComposeEmailMsgForDLForwardToHRHead(ICollection<OrderItemBriefDto> OrderItems, EmployeeDto senderObj, EmployeeDto recipient);
    }
}