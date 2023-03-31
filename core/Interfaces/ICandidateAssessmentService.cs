using core.Dtos;
using core.Entities.HR;
using core.Params;


namespace core.Interfaces
{
     public interface ICandidateAssessmentService
    {
        Task<CandidateAssessmentWithErrorStringDto> AssessNewCandidate(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId);
        Task<CandidateAssessment> GetNewAssessmentObject(bool requireInternalReview, int candidateId, int orderItemId, int loggedInIdentityUserId);
        Task<ICollection<CandidateAssessedDto>> GetAssessedCandidatesApproved();
        Task<Pagination<CandidateAssessedDto>> GetShortlistedPaginated(CVRefParams cvrefParams);
        Task<MessagesDto> EditCandidateAssessment(CandidateAssessment candidateAssessment, int loggedinEmployeeId, string employeeName);
        Task<bool> DeleteCandidateAssessment(int CandidateAssessmentId);
        Task<bool> DeleteCandidateAssessmentItem(CandidateAssessmentItem assessmentItem);
        Task<CandidateAssessment> GetCandidateAssessment(int candidateId, int orderItemId);   
        Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentAndChecklist(int candidateId, int orderItemId, int loggedInEmployeeId);
        Task<ICollection<AssessmentsOfACandidateIdDto>> GetCandidateAssessmentHeaders(int candidateid);
        
    }
}