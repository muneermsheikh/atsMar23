using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class OrderItemsAndAgentsToFwdDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ProjectManagerId {get; set;}
        public ICollection<OrderItemToFwdDto> Items {get; set;}
        public ICollection<AssociateToFwdDto> Agents {get; set;}
        public DateTime DateForwarded {get; set;}
    }
}