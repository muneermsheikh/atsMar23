using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class InterviewDto
    {
		public InterviewDto()
		{
		}

		public InterviewDto(int orderId, int orderNo, DateTime orderDate, int customerId, string customerName, 
            ICollection<InterviewItemDto> interviewItems)
		{
			OrderId = orderId;
			OrderNo = orderNo;
			OrderDate = orderDate;
			CustomerId = customerId;
			CustomerName = customerName;
			InterviewItems = interviewItems;
		}

		public int Id {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate {get; set;}
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewVenue { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto { get; set; }
        public int InterviewLeaderId { get; set; }
        public string CustomerRepresentative { get; set; }
        public string InterviewStatus {get; set;} = "open";
        public string ConcludingRemarks {get; set;}
        public ICollection<InterviewItemDto> InterviewItems {get; set;}
 
    }
}