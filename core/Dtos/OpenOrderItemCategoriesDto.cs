using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class OpenOrderItemCategoriesDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo {get; set;}        
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryRefAndName {get; set;}
        public int Quantity { get; set; }
    }
}