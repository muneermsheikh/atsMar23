using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;

namespace core.Interfaces
{
     public interface IAssessmentQBankService
    {
        Task<ICollection<Category>> GetExistingCategoriesInAssessmentQBank();  
        Task<ICollection<AssessmentQBank>> GetAssessmentQBanks();
        Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName);
        Task<ICollection<OrderItemAssessmentQ>> GetAssessmentQBankByCategoryId(int orderitemid, int id);
        Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model);
        Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model);
    }
}