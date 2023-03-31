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
			OrderReviewStatus = orderReviewStatus;
		}

		public int ReviewItemStatusId {get; set;}
        public int OrderReviewStatus { get; set; }
    }
}