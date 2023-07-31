using api.Errors;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    public class AssessmentQBankController : BaseApiController
    {
        private readonly IAssessmentQBankService _service;

        public AssessmentQBankController(IAssessmentQBankService service)
        {
            _service = service;
        }

        [HttpGet("existingqbankcategories")]
        public async Task<ICollection<Category>> ExistingQBankCategories()
        {
            return await _service.GetExistingCategoriesInAssessmentQBank();
        }

        [HttpGet("assessmentbankqs")]
        public async Task<ICollection<AssessmentQBank>> GetAssessmentBankQs()
        {
            return await _service.GetAssessmentQBanks();
        }

        [HttpGet("catqs/{categoryName}")]
        public async Task<AssessmentQBank> GetAssessmentQsOfCategoryByName(string categoryName)
        {
            return await _service.GetAssessmentQsOfACategoryByName(categoryName);
        }

        [HttpGet("catqsbycategoryid/{orderitemid}/{id}")]
        public async Task<ICollection<OrderItemAssessmentQ>> GetAssessmentQBankByCategoryId(int orderitemid, int id)
        {
            var q = await _service.GetAssessmentQBankByCategoryId(orderitemid, id);
            return q;
        }

        
        [HttpPost]
        [Authorize] //(Roles ="Admin, HRManager, HRSupervisor")]
        public async Task<ActionResult<AssessmentQBank>> InsertAssessmentQ(AssessmentQBank qbank)
        {
            var q = await _service.InsertAssessmentQBank(qbank);
            if (q == null) return BadRequest(new ApiResponse(400, "Bad Request - this probably means the Assessment Question for the chosen category already exists"));
            return Ok(q);
        }
        [Authorize] //(Roles ="Admin, HRManager, HRSupervisor")]
        [HttpPut]
        public async Task<AssessmentQBank> UpdateAssessmentQ(AssessmentQBank qBank)
        {
            var success = await _service.UpdateAssessmentQBank(qBank);
            return success;
        }

    }
}