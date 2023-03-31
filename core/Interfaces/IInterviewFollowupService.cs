using core.Dtos;
using core.Entities.HR;

namespace core.Interfaces
{
     public interface IInterviewFollowupService
    {
        Task<ICollection<InterviewItemCandidateFollowup>> AddToInterviewItemCandidatesFollowup(InterviewCandidateFollowupToAddDto followups);
        Task<ICollection<InterviewItemCandidateFollowup>> EditInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups);
        Task<bool> DeleteInterviewItemCandidatesFollowup(ICollection<InterviewItemCandidateFollowup> followups);

    }
}