using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.Orders;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class OrderItemService : IOrderItemService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly IMapper _mapper;
          public OrderItemService(ATSContext context, IUnitOfWork unitOfWork, IMapper mapper)
          {
               _mapper = mapper;
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<OrderItemBriefDto> GetOrderItemRBriefDtoFromOrderItemId(int OrderItemId)
          {
               var qry = await(from i in _context.OrderItems where i.Id == OrderItemId
                    join o in _context.Orders on i.OrderId equals o.Id 
                    select new OrderItemBriefDto{
                         OrderItemId = OrderItemId, RequireInternalReview = i.RequireInternalReview,
                         OrderId = o.Id, OrderNo = o.OrderNo, CustomerName = o.Customer.CustomerName,
                         OrderDate = o.OrderDate, CategoryId = i.CategoryId, CategoryName = i.Category.Name,
                         CategoryRef = o.OrderNo + "-" + i.SrNo, Quantity=i.Quantity, Status=i.Status,
                         JobDescription = i.JobDescription, Remuneration = i.Remuneration })
                         .FirstOrDefaultAsync();
                    

               if (qry == null) return null;
               
               return qry;
          }

          public async Task<ICollection<OrderItemBriefDto>> GetOrderItemsBriefFromOrderItemIds(List<int>  OrderItemIds)
          {
               var qry =  _context.OrderItems.Where(x => OrderItemIds.Contains(x.Id)).AsQueryable();

               var orderItems = await qry.ProjectTo<ICollection<OrderItemBriefDto>>(_mapper.ConfigurationProvider).ToListAsync();
               
               if (orderItems == null) return null;
               
               return (ICollection<OrderItemBriefDto>)orderItems;
          }
          
          public async Task<OrderItemBriefDto> GetOrderItemBriefDtoFromOrderItem(OrderItem orderItem)
          {
               return await ConvertOrderItemToBriefDto(orderItem);
               
          }

          private async Task<OrderItemBriefDto> ConvertOrderItemToBriefDto(OrderItem orderItem)
          {
                if (orderItem == null) return null;

               var dto = _mapper.Map<OrderItem, OrderItemBriefDto>(orderItem);
            
               var details = await (from i in _context.OrderItems where i.Id == orderItem.OrderId
                    join o in _context.Orders on i.OrderId equals o.Id
                    join cust in _context.Customers on o.CustomerId equals cust.Id
                    join c in _context.Categories on i.CategoryId equals c.Id
                    select new {OrderNo=i.OrderNo, CategoryName=c.Name, CustomerName = cust.CustomerName, OrderDate = o.OrderDate})
                    .FirstOrDefaultAsync();
                dto.CategoryName = details.CategoryName;
                dto.CategoryRef = details.OrderNo + "-"+ orderItem.SrNo;
                dto.CategoryName = dto.CategoryName + "(" + dto.CategoryRef + ")";
                dto.CustomerName = details.CustomerName;
                dto.OrderDate = details.OrderDate;
                dto.Status = orderItem.Status;
                dto.OrderItemId = orderItem.Id;
                dto.OrderNo=details.OrderNo;

                return dto;
          }
     }
}