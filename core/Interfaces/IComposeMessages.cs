using core.Dtos;
using core.Entities.HR;
using core.Entities.Orders;

namespace core.Interfaces
{
     public interface IComposeMessages
    {
       public string GetSelectionDetails(string CandidateName, int ApplicationNo, string CustomerName, string CategoryName, Employment employmt);
       string ComposeOrderItems(int orderNo, ICollection<OrderItem> orderItems, bool hasException);
       string GetSelectionDetailsBySMS(SelectionDecision selection);
       Task<string> TableOfOrderItemsContractReviewedAndApproved(ICollection<int> itemIds);
       Task<OrderItemReviewStatusDto> CumulativeCountForwardedSoFar(int orderitemId);
       Task<EnumCandidateAssessmentResult> AssessmentGrade(int candidateId, int orderitemId);
       Task<string> TableOfCVsSubmittedByHRExecutives(ICollection<CVsSubmittedDto> cvsSubmitted);
       Task<string> TableOfCVsSubmittedByHRSup(ICollection<CVsSubmittedDto> cvsSubmitted);
       Task<string> TableOfRelevantOpenings(List<int> Ids);
    }
}