using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
     public class RemunerationSpecs: BaseSpecification<Remuneration>
    {
        public RemunerationSpecs(RemunerationSpecParams specParams)
            : base(x => 
                (!specParams.OrderId.HasValue || x.OrderId == specParams.OrderId) &&
                (!specParams.OrderItemId.HasValue || x.OrderItemId == specParams.OrderItemId) &&
                (!specParams.OrderNo.HasValue || x.OrderNo == specParams.OrderNo)
                )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderBy(x => x.OrderItemId);
        }

        public RemunerationSpecs(int orderItemId) 
            : base(x => x.OrderItemId == orderItemId)
        {
            AddOrderBy(x => x.OrderItemId);
        }
        
      }
}