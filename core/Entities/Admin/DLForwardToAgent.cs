using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class DLForwardToAgent: BaseEntity
    {
        
        public DLForwardToAgent()
        {
        }

        public DLForwardToAgent(int orderid, int orderno, DateTime orderdate, int customerid, string customername)
        {
            OrderId = orderid;
        }
        
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string customerName {get; set;}
        public string CustomerCity {get; set;}
        public int ProjectManagerId {get; set;}
        public ICollection<DLForwardCategory> DlForwardCategories {get; set;}
    }
}