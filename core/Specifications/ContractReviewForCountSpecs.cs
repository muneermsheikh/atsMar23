using core.Entities.Orders;

namespace core.Specifications
{
     public class ContractReviewForCountSpecs: BaseSpecification<ContractReview>
    {
        public ContractReviewForCountSpecs(ContractReviewSpecParams cParams)
            : base(x => 
                (!cParams.OrderId.HasValue || x.OrderId == cParams.OrderId)
                //&& (!cParams.OrderItemId.HasValue || x.OrderItemId == cParams.OrderItemId)
                && (cParams.OrderIdInts.Count > 0) || cParams.OrderIdInts.Contains(x.OrderId)
                && (!cParams.ReviewStatus.HasValue || x.RvwStatusId == (int)cParams.ReviewStatus)
            )
        {
        }

       

      }
}