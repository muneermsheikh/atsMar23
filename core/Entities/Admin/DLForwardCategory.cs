using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class DLForwardCategory: BaseEntity
    {
        public DLForwardCategory()
		{
		}

		public DLForwardCategory(int orderId, int orderItemId, int categoryId, string categoryName)
		{
			OrderId = orderId;
			OrderItemId = orderItemId;
			
			CategoryId = categoryId;
			CategoryName = categoryName;
		}

		public int OrderId {get; set;}
		public int OrderItemId {get; set;}
		public int DLForwardToAgentId { get; set; }
		public int CategoryId {get; set;}
		public string CategoryName {get; set;}
		public int Charges {get; set;}
		public ICollection<DLForwardCategoryOfficial> DlForwardCategoryOfficials {get; set;}
 
    }
}