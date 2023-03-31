using core.Dtos;
using core.Entities;
using core.Entities.Admin;
using core.Entities.MasterEntities;
using core.Params;

namespace core.Interfaces
{
     public interface IMastersService
    {
        Task<Category> AddCategory(string categoryName);
        Task<bool> EditCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Category category);
        Task<Pagination<Category>> GetCategoryListAsync(CategorySpecParams categoryParams);
        Task<ICollection<Category>> GetCategoriesAsync();
        
        Task<int> CategoryFromOrderItemId (int orderItemId);

    //industry
        Task<Industry> AddIndustry(string industryName);
        Task<bool> EditIndustryAsync(Industry industry);
        Task<bool> DeleteIndustryyAsync(Industry industry);
        Task<Industry> GetIndustry(int id);
        Task<Pagination<Industry>> GetIndustryListAsync(IndustrySpecParams industryParams);
        Task<ICollection<Industry>> GetIndustryListWOPaginationAsync();
        //Task<IReadOnlyList<Industry>> GetIndustryListOfACategoryAsync(IndustryParams industryParams);
        
    //Qualifications
    
        Task<Qualification> AddQualification(string qualificationName);
        Task<bool> EditQualificationAsync(Qualification qualification);
        Task<bool> DeleteQualificationAsync(Qualification qualification);
        Task<ICollection<Qualification>> GetQualificationsAsync();
        Task<Pagination<Qualification>> GetQualificationPaginated(QualificationSpecParams specParams);

    //ReviewItemStatus.Description for Contract REVIEW RESULTS
        Task<ReviewItemData> AddReviewItemData(string reviewDescriptionName, bool isBoolean);
        Task<bool> EditReviewItemDataAsync(ReviewItemData reviewItemData);
        Task<bool> DeleteReviewItemDataAsync(ReviewItemData reviewItemData);
        Task<IReadOnlyList<ReviewItemData>> GetReviewItemDataDescriptionListAsync();

 //ReviewItemStatus.Description for Contract REVIEW RESULTS
        Task<ReviewItemStatus> AddReviewItemStatus(string reviewDescriptionName);
        Task<bool> EditReviewItemStatusAsync(ReviewItemStatus ReviewItemStatus);
        Task<bool> DeleteReviewItemStatusAsync(ReviewItemStatus ReviewItemStatus);
        Task<IReadOnlyList<ReviewItemStatus>> GetReviewItemStatusListAsync();

    //ReviewItemData.Description for Contract REVIEW RESULTS
        Task<SkillData> AddSkillData(string skillname);
        Task<bool> EditSkillDataAsync(SkillData skillData);
        Task<bool> DeleteSkillDataAsync(SkillData skillData);
        Task<IReadOnlyList<SkillData>> GetSkillDataListAsync();

    //Help
        Task<Help> GetHelpForATopic(string topic);
        
    }
}