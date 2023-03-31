using core.Entities.EmailandSMS;

namespace core.Interfaces
{
     public interface IComposeMsgsForInterview
    {
        Task<EmailMessage> ComposeEmailMsgToCandidateForInterviewInfo(int interviewItemId, int candidateId);
        Task<SMSMessage> ComposeSMSToCandidateForInterviewInfo(int interviewItemId, int candidateId);
    }
}