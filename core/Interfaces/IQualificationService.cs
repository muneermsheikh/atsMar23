using core.Dtos;
using core.Entities.MasterEntities;
using core.Params;

namespace core.Interfaces
{
     public interface IQualificationService
    {
        Task<Qualification> AddQualification(string qualificationName);
        Task<bool> EditQualificationAsync(Qualification qualification);
        Task<bool> DeleteQualificationAsync(Qualification qualification);
        Task<ICollection<Qualification>> GetListAsync();
        Task<Pagination<Qualification>> GetQualificationPaginated(QualificationSpecParams specParams);
    }
}