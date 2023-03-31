using core.Dtos;
using core.Entities.HR;

namespace core.Interfaces
{
     public interface ICommonServices
    {
     //order and order items
         Task<string> CategoryNameFromCategoryId(int categoryId);
         Task<string> CustomerNameFromOrderDetailId(int orderDetailId);
         Task<string> CustomerNameFromCustomerId(int customerId);
    //employees
         Task<string> GetEmployeeNameFromEmployeeId(int id);
         Task<string> GetEmployeePositionFromEmployeeId(int employeeId);
 
         Task<CustomerBriefDto> CustomerBriefDetailsFromCustomerId(int customerId);
         Task<string> DeploymentStageNameFromStageId(int stageId);
         Task<CommonDataDto> CommonDataFromCVRefId(int cvrefid);
         Task<CommonDataDto> CommonDataFromOrderDetailIdAndCandidateId(int CVReviewId);
         Task<CommonDataDto> CommonDataFromOrderDetailIdAndCandidateId(int OrderItemId, int CandidateId);
         Task<CommonDataForCVRefDto> CommonDataForCVRefFromOrderItemAndCandidateId(int OrderItemId, int candidateId);
         Task<CommonDataDto> CommonDataFromOrderItemCandidateIdWithChecklistId(int OrderItemId, int candidateId);
         Task<ICollection<SelectionDecisionToRegisterDto>> PopulateSelectionDecisionsToRegisterDto(ICollection<SelectionDecisionToRegisterDto> dto);
         Task<Employment> PopulateEmploymentFromCVRefId(int cvrefid, int salary, int charges, DateTime selectedOn);
         Task<CommonDataDto> PendingDeployments();
         //Task<OrderAssignmentDto> GetOrderAssignmentDto(int orderId);
         Task<string> CategoryRefFromOrderItemId(int OrderItemId);  
         Task<string> CandidateNameFromCandidateId(int CandidateId);
    }
}