using core.Dtos;
using core.Entities.Orders;

namespace core.Interfaces
{
     public interface IOrderItemService
    {
        Task<OrderItemBriefDto> GetOrderItemBriefDtoFromOrderItem(OrderItem orderItem);
        Task<OrderItemBriefDto> GetOrderItemRBriefDtoFromOrderItemId(int OrderItemId);
        Task<ICollection<OrderItemBriefDto>> GetOrderItemsBriefFromOrderItemIds(List<int>  OrderItemIds);
    }
}