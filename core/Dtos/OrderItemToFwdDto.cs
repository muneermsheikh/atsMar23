using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Admin;
using core.Entities.Orders;

namespace core.Dtos
{
    public class OrderItemToFwdDto
    {
        public int Id {get; set;}
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        
        public int CategoryId { get; set; }
        public string CategoryRef { get; set; }
        public string CategoryName {get; set;}
        public int Quantity {get; set;}
        public ICollection<DLForwardCategoryOfficial> DLForwardDates { get; set; }
    }
}