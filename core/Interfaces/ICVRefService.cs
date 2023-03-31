using core.Dtos;
using core.Entities.HR;

namespace core.Interfaces
{
     public interface ICVRefService
    {
        Task<ICollection<CVRef>> GetReferralsOfOrderItemId(int orderItemId);
        Task<ICollection<CVRef>> GetReferralsOfACandidate(int candidateId);
        Task<ICollection<CustomerReferralsPendingDto>> CustomerReferralsPending(int userId);
        Task<CVRef> GetReferralById(int cvrefid);
        Task<CVRef> GetReferralByCandidateAndOrderItem(int candidateId, int orderItemId);
        Task<MessagesDto> MakeReferralsAndCreateTask (ICollection<int> CVReviewIds, LoggedInUserDto loggedInUserDto);
        Task<bool> EditReferral (CVRef cvref);
        Task<bool> DeleteReferral (CVRef cvref);
    }
}