using api.Errors;
using api.Extensions;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Interfaces;
using core.Dtos;
using core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
     public class ContractReviewController : BaseApiController
    {
        private readonly IContractReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ContractReviewController(IContractReviewService reviewService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _reviewService = reviewService;
        }

        [HttpGet("reviewitemstatus")]
        public async Task<ActionResult<ICollection<ReviewItemStatus>>> GetReviewItemStatus ()
        {
            var st = await _reviewService.GetReviewItemStatus();
            if(st==null) return NotFound(new ApiResponse(404, "No Contract Review Item status on record"));
            return Ok(st);
        }

        [Authorize] //(Roles = "ContractReviewRole")]
        [HttpPost("createobject/{orderId}")]
        public async Task<ContractReview> CreateContractReviewObject(int orderId)
        {
            var loggedInAppUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            var cReview = await _reviewService.CreateContractReviewObject(orderId, loggedInAppUser.Id);
            return cReview;
        }

        [Authorize] //(Roles = "ContractReviewRole")]
        [HttpPut("update")]
        public async Task<ActionResult<EmailMessage>> UpdateContractReview(ContractReview contractReview)
        {
            var msgDto = await _reviewService.EditContractReview(contractReview);
            if (msgDto == null) {
                return BadRequest(new ApiResponse(404, "Failed to update the Contract Review"));
            } else {
                return msgDto.EmailMessage;
            }
        }

       
        [Authorize] //(Roles = "ContractReview")]
        [HttpGet("reviews")]
        public async Task<Pagination<ContractReview>> GetContractReviews([FromQuery]ContractReviewSpecParams reviewParams)
        {
            var obj = await _reviewService.GetContractReviews(reviewParams);
            return obj;
        }

         [Authorize]    //(Roles ="ContractReview")]
        [HttpGet("reviewdata")]
        public async Task<ICollection<ReviewItemData>> GetReviewItemData()
        {
            return await _reviewService.GetReviewData();
        }
        
        [Authorize] //(Roles ="ContractReview")]
        [HttpGet("{id}")]
        public async Task<ICollection<ContractReview>> GetContractReviews(int id)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            var obj = await _reviewService.GetOrAddContractReview(id,loggedInUser.loggedInEmployeeId);
            if (obj != null) {
                var rvws = new List<ContractReview>();
                rvws.Add(obj);
                return rvws;
            }
            return null;
        }


        [Authorize] //(Roles = "ContractReview")]
        [HttpGet("orderitemdto/{orderitemid}")]
        public async Task<ICollection<ContractReviewItemDto>> GetContractReviewItemsDto(ContractReviewItemSpecParams cParams)
        {
            var rvwitem = await _reviewService.GetContractReviewItemsWithOrderDetails(cParams);
            return rvwitem;
        }



        [Authorize] //(Roles = "ContractReview")]
        [HttpDelete("{orderid}")]           //deletes contractreview and all children
        public async Task<ActionResult<bool>> DeleteContractReview(int orderid)
        {
            if (!await _reviewService.DeleteContractReview(orderid))  return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

        [Authorize] //(Roles = "ContractReview")]
        [HttpDelete("item/{orderitemid}")]      //deletes contractReviewItem and all reviewitems
        public async Task<ActionResult<bool>> DeleteContractReviewItem(int orderitemid)
        {
            if (!await _reviewService.DeleteContractReviewItem(orderitemid))  return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

        [Authorize] //(Roles = "ContractReview")]
        [HttpDelete("reviewitem/{id}")]
        public async Task<ActionResult<bool>> DeleteContractReviewReviewItem(int id)
        {
            if (!await _reviewService.DeleteReviewReviewItem(id)) return BadRequest(new ApiResponse(404, "Failed to delete the record"));
            return Ok(true);
        }

         [Authorize]    //(Roles ="ContractReview")]
        [HttpGet("reviewitem/{orderitemid}")]
        public async Task<ActionResult<ContractReviewItemDto>> GetReviewResults (int orderitemid)
        {
            var results = await _reviewService.GetOrAddReviewResults(orderitemid);

            if(results !=null) return Ok(results);

            return NotFound(new ApiResponse(404, "Not found"));
        }

        [Authorize] //(Roles ="ContractReview")]
        [HttpPut("reviewitem")]
        public async Task<ActionResult<ContractReviewItemReturnValueDto>> UpdateContractReviewItem(ContractReviewItemDto model)
        {
            var loggedInUser = await _userManager.FindByEmailFromClaimsPrincipal(User);
            var result = await _reviewService.EditContractReviewItem(model, loggedInUser.loggedInEmployeeId);

            if (result==null) return BadRequest(new ApiResponse(402, "failed to update the contract review item"));

            return Ok(result);
        }
        
    }
}