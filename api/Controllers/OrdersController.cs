using api.Errors;
using api.Extensions;
using AutoMapper;
using core.Dtos;
using core.Entities.Identity;
using core.Entities.Orders;
using core.Interfaces;
using core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     [Authorize]
     public class OrdersController : BaseApiController
     {
          private readonly IOrderService _orderService;
          private readonly IMapper _mapper;
          private readonly IGenericRepository<Order> _orderRepo;
          private readonly UserManager<AppUser> _userManager;
          private readonly IOrderItemService _orderItemService;
          private readonly IOrdersGetService _ordersGetService;
          public OrdersController(IOrderService orderService, IOrderItemService orderItemService, 
               IMapper mapper, IGenericRepository<Order> orderRepo, UserManager<AppUser> userManager,
               IOrdersGetService ordersGetService
               )
          {
               _ordersGetService = ordersGetService;
               _userManager = userManager;
               _orderRepo = orderRepo;
               _mapper = mapper;
               _orderService = orderService;
               _orderItemService = orderItemService;
          }


          //(Policy = "OrdersViewReportRole")]
          [HttpGet]
          public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrdersAll(OrdersSpecParams orderParams)
          {
               var orders = await _orderService.GetOrdersAllAsync(orderParams);
               if (orders == null) return NotFound(new ApiResponse(400, "No orders found"));

               return Ok(orders);
          }
          
          [HttpGet("ordersbriefpaginated")]
          public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrdersBriefAll([FromQuery]OrdersSpecParams orderParams)
          {
               //var orders = await _orderService.GetOrdersBriefAllAsync(orderParams);
               var orders = await _orderService.GetOrdersAllAsync(orderParams);
               if (orders == null) return NotFound(new ApiResponse(400, "No orders found"));

               return Ok(orders);
          }

          [HttpGet("openorderitemlist")]
          public async Task<ICollection<OrderItemBriefDto>> GetCurrentOpeningsDto()
          {
               return await _orderService.GetOpenOrderItemsNotPaged();
          }

          [HttpGet("openorderitemsmatchingcandidate")]
          public async Task<ICollection<OrderItemBriefDto>> GetCurrentOpeningsDtoForCandidate(int candidateId)
          {
               return await _orderService.GetOpenOrderItemsNotPaged();
          }

          [HttpGet("orderitemsbyorderid/{orderid}")]
          public async Task<ActionResult<ICollection<OrderItemBriefDto>>> GetOrderItemsByOrderId (int orderid)
          {
               var orderItems = await _orderService.GetOrderItemsBriefDtoByOrderId(orderid);

               if (orderItems !=null) return Ok(orderItems);

               return NotFound();
          }

          [HttpGet("orderbriefdto/{orderid}")]
          public async Task<ActionResult<OrderBriefDtoR>> GetOrderBrief (int orderid)
          {
               var orderItems = await _orderService.GetOrderBrief(orderid);

               if (orderItems !=null) return Ok(orderItems);

               return NotFound();
          }

          
          [HttpGet("itemdtobyid/{orderitemid}")]
          public async Task<ActionResult<ICollection<OrderItemBriefDto>>> GetOrderItemBriefByOrderItemId (int orderitemid)
          {
               var orderItems = await _orderItemService.GetOrderItemRBriefDtoFromOrderItemId(orderitemid);

               if (orderItems !=null) return Ok(orderItems);

               return NotFound();
          }

          [HttpGet("byidwithitems/{id}")]
          public async Task<ActionResult<Order>> GetOrderByIdWithItemsAsync (int id)
          {
               var order = await _orderService.GetOrderByIdWithItemsAsyc(id);

               if (order !=null) return Ok(order);

               return NotFound();
          }

          [HttpGet("byidswithitems")]
          public async Task<ActionResult<OrderBriefDtoR>> GetOrdersByIdsWithItemsAsync (ICollection<int> orderids)
          {
               var order = await _orderService.GetOrdersByIdsWithItemsAsyc(orderids);

               if (order !=null) return Ok(order);

               return NotFound();
          }

          [HttpGet("byid/{id}")]
          public async Task<ActionResult<Order>> GetOrderByIdWithItemsJDRemunertionAsyc (int id)
          {
               var order = await _orderService.GetOrderByIdWithItemsJDRemunertionAsyc(id);

               if (order !=null) return Ok(order);

               return NotFound();
          }

          [HttpGet("ordercities")]
          public async Task<ICollection<CustomerCity>> GetOrderCities ()
          {
               return (ICollection<CustomerCity>)await _orderService.GetOrderCityNames();
          }

          [HttpGet("refcodefromorderitemid/{orderitemid}")]
          public async Task<string> GetOrderRefCodeFromOrderItemId(int orderitemid)
          {
               var s = await _ordersGetService.GetOrderRefCode(orderitemid);
               if(string.IsNullOrEmpty(s)) return "undefined";
               return s;
          }


          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPost]
          public async Task<ActionResult<Order>> CreateOrder(OrderToCreateDto dto)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               //dto.LoggedInAppUserId = loggedInUser.Id;
               var order = await _orderService.CreateOrderAsync(dto);
               if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order"));

               return Ok(order);
          }

          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPost("orders")]
          public async Task<ActionResult<ICollection<Order>>> CreateOrders(ICollection<OrderToCreateDto> dtos)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);

               var order = await _orderService.CreateOrdersAsync(loggedInUser.loggedInEmployeeId, dtos);

               if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order"));

               return Ok(order);
          }

          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditOrder(Order order)
          {
               if ( await _orderService.EditOrder(order, 0)) return Ok(true);
               return BadRequest(new ApiResponse(400, "Problem updating the order"));
          }


          
          [Authorize]    //(Roles = "Admin, DocumentControllerAdmin")]
          [HttpDelete("order/{orderid}")]
          public async Task<bool> DeleteOrder(int orderid)
          {
               return await _orderService.DeleteOrder(orderid);
          }

          
          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor, HRExecutive, DocumentControllerAdmin")]
          [HttpPut("updatedlfwd")]
          public async Task<bool> UpdateOrderDLForwardedToHR(IdAndDate idanddate)
          {
               return await _orderService.UpdateDLForwardedDateToHR(idanddate.OrderId, idanddate.DateForwarded);
          }

          //remunerations
          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpPost("remun")]
          public async Task<ActionResult<Remuneration>> CreateRemuneration(Remuneration remuneration)
          {
               var remun = await _orderService.AddRemuneration(remuneration);
               if (remun == null) return BadRequest(new ApiResponse(400, "failed to save the remuneration detail"));

               //return Ok(_mapper.Map<Remuneration, RemunerationDto>(remun));
               return Ok(remun);
          }

          [HttpGet("jd/{orderitemid}")]
          public async Task<ActionResult<JDDto>> GetOrCreateJD(int orderitemid)
          {
               var jd = await _orderService.GetOrAddJobDescription(orderitemid);

               if (jd == null) return BadRequest(new ApiResponse(404, "Failed to get or create job description"));

               return Ok(jd);

          }

          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor, DocumentControllerAdmin")]
          [HttpGet("remuneration/{orderitemid}")]
          public async Task<ActionResult<RemunerationFullDto>> GetOrCreateRemuneration(int orderitemid)
          {
               var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
               var remun = await _orderService.GetOrAddRemunerationByOrderItemAsync(orderitemid);

               if (remun == null) return BadRequest(new ApiResponse(404, "Failed to get or create the remuneration details"));

               return Ok(remun);

          }


          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPut("jd")]
          public async Task<ActionResult<bool>> UpdateJD (JDDto jddto)
          {
               return await _orderService.EditJobDescription(jddto);
          }

          
          [Authorize]    //(Roles = "Admin, HRManager, HRSupervisor")]
          [HttpPut("remuneration")]
          public async Task<ActionResult<bool>> UpdateRemuneration (RemunerationFullDto dto)
          {
               return await _orderService.EditRemuneration(dto);
          }
          
     }
}