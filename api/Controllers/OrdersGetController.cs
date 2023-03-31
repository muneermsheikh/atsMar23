using core.Dtos;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     public class OrdersGetController : BaseApiController
    {
        private readonly ILogger<OrdersGetController> _logger;
            private readonly IOrdersGetService _service;

        public OrdersGetController(ILogger<OrdersGetController> logger, IOrdersGetService service)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet("openorderitemcategorylist")]
        public async Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenItemCategoriesDto()
        {
            return await _service.GetOpenOrderIemCategories();
        }

    }
}