using core.Entities.MasterEntities;

namespace core.Interfaces
{
     public interface ICategoryRepository
    {
        Task<Category> GetCategryBIdAsync(int id);
        Task<IReadOnlyList<Category>> GetCategoryList();
    }
}