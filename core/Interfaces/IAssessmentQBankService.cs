using core.Entities.MasterEntities;

namespace core.Interfaces
{
     public interface IAssessmentQBankService
    {
        Task<ICollection<Category>> GetExistingCategoriesInAssessmentQBank();  
        Task<ICollection<AssessmentQBank>> GetAssessmentQBanks();
        Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName);
        Task<AssessmentQBank> GetAssessmentQBankByCategoryId(int id);
        Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model);
        Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model);
    }
}