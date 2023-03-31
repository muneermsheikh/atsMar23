using core.Dtos;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Params;

namespace core.Interfaces
{
     public interface ISelectionDecisionService
    {
        Task<SelectionMsgsAndEmploymentsDto> RegisterSelections(SelDecisionsToAddParams selDto, int loggedInEmpId, string loggedUserName);
         Task<bool> EditSelection(SelectionDecision selectionDecision);
         Task<bool> DeleteSelection(int id);
         Task<Pagination<SelectionDecision>> GetSelectionDecisions (SelDecisionSpecParams specParams);
         Task<Pagination<SelectionsPendingDto>> GetPendingSelections(CVRefSpecParams refParams);
         Task<ICollection<SelectionStatus>> GetSelectionStatus();
         Task<ICollection<EmailMessage>> ComposeSelectionEmailMessagesFromCVRefIds(ICollection<int> cvrefids, int loggedinEmpId, string loggedInUsername, DateTime datetimenow);
         Task<ICollection<EmailMessage>> ComposeRejEmailMessagesFromCVRefIds(ICollection<int> cvrefids, int loggedinEmpId, string loggedInUsername, DateTime datetimenow);
         
    }
}