using core.Dtos.Admin;

namespace core.Entities.Admin
{
    public class DLForwardedDateDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public ICollection<ForwardedCategoryDto> ForwardedCategories { get; set; }
    }
}