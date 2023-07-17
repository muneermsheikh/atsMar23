using core.Dtos;
using core.Entities.EmailandSMS;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     public class OrdersGetController : BaseApiController
    {
        private readonly ILogger<OrdersGetController> _logger;
        private readonly IOrdersGetService _service;
        private readonly IOrderService _orderService;

        public OrdersGetController(ILogger<OrdersGetController> logger, IOrderService orderService, IOrdersGetService service)
        {
            _service = service;
            _logger = logger;
            _orderService = orderService;
        }


        [HttpGet("ackToClient/{orderid}")]
        public async Task<bool> GenerateOrderAcknowledgement(int orderid)
        {
            return  await _orderService.ComposeMsg_AckToClient(orderid);
        }

        [HttpGet("openorderitemcategorylist")]
        public async Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenItemCategoriesDto()
        {
            return await _service.GetOpenOrderIemCategories();
        }

    }
}