using core.Entities.Orders;
using core.Params;

namespace core.Specifications
{
     public class RemunerationForCountSpecs: BaseSpecification<Remuneration>
    {
        public RemunerationForCountSpecs(RemunerationSpecParams rParams)
            : base(x => 
                (!rParams.OrderId.HasValue || x.OrderId == rParams.OrderId) &&
                (!rParams.OrderItemId.HasValue || x.OrderItemId == rParams.OrderItemId) &&
                (!rParams.OrderNo.HasValue || x.OrderNo == rParams.OrderNo)
                )
        {
        }

        public RemunerationForCountSpecs(int orderItemId) 
            : base(x => x.OrderItemId == orderItemId)
        {
        }
        
      }
}