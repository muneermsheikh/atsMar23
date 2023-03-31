using core.Entities.EmailandSMS;
using core.Entities.Tasks;

namespace core.Interfaces
{
     public interface IOrderAssignmentService
    {
        Task<ICollection<EmailMessage>> DesignOrderAssessmentQs(int orderId, int loggedInEmployeeId);
        Task<bool> DeleteHRExecAssignment(int orderItemId);
        Task<bool> OrderItemsNeedAssessment(int orderId);
        Task<bool> EditOrderAssignment(ApplicationTask task);
        Task<bool> SetTaskAsCompleted(int id, string remarks);
        Task<bool> DeleteApplicationTask(int id, string remarks);
        
    }
}