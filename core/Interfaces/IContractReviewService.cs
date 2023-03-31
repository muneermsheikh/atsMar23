using core.Dtos;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Specifications;

namespace core.Interfaces
{
     public interface IContractReviewService
    {
        Task<Pagination<ContractReview>> GetContractReviews(ContractReviewSpecParams cParams);
        Task<ContractReview> GetOrAddContractReview(int id, int loggedInEmployeeid);
        Task<ContractReview> CreateContractReviewObject(int orderId, string AppUserId);
        //Task<bool> UpdateContractReview(ContractReview cReview);
        Task<ContractReview> GetContractReviewDtoByOrderIdAsync(int orderId);
        //Task<IReadOnlyList<ContractReviewItemDto>> GetContractReviewItemsByOrderIdAsync(int orderid);
        //Task<ContractReview> GetContractReview(int orderId);
        Task<EmailMessageDto> EditContractReview (ContractReview contractReview);
        Task<ContractReviewItemReturnValueDto> EditContractReviewItem(ContractReviewItemDto model, int loggedInEmployeeId);
        Task<bool> DeleteContractReview(int orderid);
        Task<bool> DeleteContractReviewItem(int orderitemid);
        Task<bool> DeleteReviewReviewItem(int id);

        //Task<ContractReview> GetContractReview(int orderId);
        //void EditContractReview (ContractReview review);
        Task<ICollection<ReviewItemData>> GetReviewData();
        void AddReviewStatus (string reviewStatusName);
        void AddReviewItemStatus (string reviewItemStatusName);
        Task<ICollection<ReviewStatus>> GetReviewStatus();
        Task<ICollection<ReviewItemStatus>> GetReviewItemStatus();
        Task<ICollection<ContractReviewItemDto>> GetContractReviewItemsWithOrderDetails(ContractReviewItemSpecParams cParams);
        Task<ContractReviewItemDto> GetOrAddReviewResults(int orderitemid);
        Task<int> UpdateOrderReviewStatusBasedOnOrderItemReviewStatus(int orderid, int loggedInEmployeeId);
    }
}