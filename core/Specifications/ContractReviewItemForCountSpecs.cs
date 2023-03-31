using core.Entities.Orders;

namespace core.Specifications
{
     public class ContractReviewItemForCountSpecs: BaseSpecification<ContractReviewItem>
    {
        public ContractReviewItemForCountSpecs(ContractReviewItemSpecParams cParams)
            : base(x => 
                (cParams.OrderItemIds.Count == 0 || cParams.OrderItemIds.Contains(x.OrderItemId))
                && (!cParams.ReviewItemStatus.HasValue || x.ReviewItemStatus == (int)cParams.ReviewItemStatus)
            )
        {
        }

       

      }
}