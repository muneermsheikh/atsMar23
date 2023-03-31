using core.Dtos;

namespace core.Interfaces
{
     public interface IVerifyService
    {
         Task<ICollection<string>> OrderItemIdAndCandidateIdExist(int purpose, ICollection<CandidateAndOrderItemIdDto> canandorderitemids);
        
    }
}