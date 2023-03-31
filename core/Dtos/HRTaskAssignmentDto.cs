using System.Collections.Generic;

namespace core.Dtos
{
    public class HRTaskAssignmentDto
    {
        public int HRExecutiveId { get; set; }
        public ICollection<int> OrderItemIds { get; set; }
    }
    
}