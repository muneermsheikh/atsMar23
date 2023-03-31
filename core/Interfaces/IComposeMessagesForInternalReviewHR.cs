using core.Dtos;
using core.Entities.EmailandSMS;

namespace core.Interfaces
{
     public interface IComposeMessagesForInternalReviewHR
    {
        Task<ICollection<EmailMessage>> ComposeMessagesToHRExecToSourceCVs(ICollection<OrderAssignmentDto> orderItemIds);
        Task<ICollection<EmailMessage>> ComposeMessagesToHRSupToReviewCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn);
        Task<ICollection<EmailMessage>> ComposeMessagesToHRMToReviewCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn);
        Task<EmailMessage> ComposeHTMLToPublish_CVSubmittedToHRSup(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId);
        Task<EmailMessage> ComposeHTMLToPublish_CVReviewedByHRSup(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId);
        Task<EmailMessage> ComposeHTMLToPublish_CVReviewedByHRManager(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId);
    }
}