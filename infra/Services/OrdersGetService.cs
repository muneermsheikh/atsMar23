using core.Dtos;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class OrdersGetService : IOrdersGetService
	{
		private readonly ATSContext _context;
        public OrdersGetService(ATSContext context )
		{
            _context = context;
		}

		public async Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenOrderIemCategories()
		{
			    var  q = (from i in _context.OrderItems where i.ReviewItemStatusId == 1 && i.Status != "Concluded"
                    join o in _context.Orders on i.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    join cat in _context.Categories on i.CategoryId equals cat.Id
                    select new OpenOrderItemCategoriesDto {
                         OrderItemId = i.Id, OrderId = o.Id, 
                         CustomerName = c.CustomerName, OrderDate = o.OrderDate,
                         CategoryId = i.CategoryId, 
                         Quantity = i.Quantity,
                         CategoryRefAndName = o.OrderNo + "-" + i.SrNo + cat.Name,
                    }).AsQueryable();

               var qry = await q.ToListAsync();
               
               return qry;

		}
	}
}