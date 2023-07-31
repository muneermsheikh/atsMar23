using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ContractReviewItemReturnValueDto
    {
		public ContractReviewItemReturnValueDto()
		{
		}

		public ContractReviewItemReturnValueDto(int reviewItemSttusId, int orderReviewStatus)
		{
			ReviewItemStatusId = reviewItemSttusId;
			OrderReviewStatusId = orderReviewStatus;
		}

		public string OrderStatus { get; set; }
        public int ContractReviewStatusId { get; set; }
        public int ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public int OrderReviewStatusId { get; set; }
		public int ReviewItemStatusId {get; set;}
    }
}