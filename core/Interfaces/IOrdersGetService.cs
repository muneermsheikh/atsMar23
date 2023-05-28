using core.Dtos;

namespace core.Interfaces
{
     public interface IOrdersGetService
    {
        Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenOrderIemCategories();
        Task<string> GetOrderRefCode(int orderitemid);
        
    }
}