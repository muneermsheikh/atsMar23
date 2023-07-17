using core.Dtos;
using core.Entities.EmailandSMS;
using core.Entities.Orders;
using core.Params;

namespace core.Interfaces
{
     public interface IOrderService
        {
            Task<Order> CreateOrderAsync(OrderToCreateDto dto);
            Task<ICollection<Order>> CreateOrdersAsync(int loggedInUserId, ICollection<OrderToCreateDto> dtos);
            Task<bool> EditOrder(Order order, int loggedInUserId);
            Task<bool> DeleteOrder(int orderid);
            Task<Pagination<OrderBriefDto>> GetOrdersAllAsync(OrdersSpecParams orderSpecParams);
            Task<Pagination<OrderBriefDto>> GetOrdersBriefAllAsync(OrdersSpecParams orderParams);
            Task<bool> OrderForwardedToHRDept(int orderId);
            Task<ICollection<CustomerCity>> GetOrderCityNames();
            Task<Order> GetOrderByIdWithItemsJDRemunertionAsyc (int id);
            Task<Order> GetOrderByIdWithItemsAsyc (int id);
            Task<ICollection<OrderBriefDtoR>> GetOrdersByIdsWithItemsAsyc (ICollection<int> ids);
            Task<OrderBriefDtoR> GetOrderBrief(int OrderId);
            //Task<ICollection<OrderBriefDtoR>> GetOpenOrdersBrief();

            Task<bool> ComposeMsg_AckToClient(int orderid);
            
        //order items
            Task<IReadOnlyList<OrderItem>> GetOrderItemsByOrderIdAsync(int OrderId);
            Task<ICollection<OrderItemBriefDto>> GetOrderItemsBriefDtoByOrderId(int OrderId);
            Task<OrderItem> GetOrderItemByOrderItemIdAsync(int Id);
            
            void AddOrderItem(OrderItem orderItem);
            void EditOrderItem(OrderItem orderItem);
            bool EditOrderItemWithNavigationObjects(OrderItem modelItem);
            Task<bool> DeleteOrderItem (OrderItem orderItem);
            Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsNotPaged();
            Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsForCandidate(int candidateId);
            

        //Job descriptions
            Task<ICollection<JobDescription>> GetJobDescriptionsByOrderIdAsync(int Id);
            Task<JDDto> GetOrAddJobDescription(int Id);
            
            void AddJobDescription(JobDescription jobDescription);
            Task<bool> EditJobDescription(JDDto dto);
            void DeleteJobDescription (JobDescription jobDescription);

    // Remuneations
            Task<IReadOnlyList<Remuneration>> GetRemunerationsByOrderIdAsync(int Id);
            Task<RemunerationFullDto> GetOrAddRemunerationByOrderItemAsync(int OrderItemId);
            
            Task<Remuneration> AddRemuneration(Remuneration remuneration);
            Task<bool> EditRemuneration(RemunerationFullDto dto);
            Task<bool> DeleteRemuneration (Remuneration remuneration);
    
    // forwardedtOHRO
            Task<bool> UpdateDLForwardedDateToHR(int orderid, DateTime forwardedToHROn);
            
    }
}