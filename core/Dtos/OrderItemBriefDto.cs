using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Orders;

namespace core.Dtos
{
    public class OrderItemBriefDto
    {
        public int Id {get; set;}
        public int OrderItemId { get; set; }
        public bool RequireInternalReview { get; set; }
        public int OrderId { get; set; }
        public int OrderNo {get; set;}
        public string CustomerName { get; set; }
        public string AboutEmployer {get; set;}
        public DateTime OrderDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryRef {get; set;}
        public string CategoryRefAndName {get; set;}
        public int Quantity { get; set; }
        public string Status { get; set; }
        public JobDescription JobDescription {get; set;}
        public Remuneration Remuneration {get; set;}
        public bool AssessmentQDesigned {get; set;}
    }
}