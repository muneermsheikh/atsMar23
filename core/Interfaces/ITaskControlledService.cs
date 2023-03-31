using core.Dtos;
using core.Entities.EmailandSMS;
using core.Entities.Tasks;

namespace core.Interfaces
{
     public interface ITaskControlledService
    {
        Task<ICollection<EmailMessage>> CreateTaskForHRExecOnOrderItemIds(ICollection<OrderAssignmentDto> assignments, int loggedInUserEmployeeId);
        Task<ICollection<TaskType>> GetTaskTypes();

        Task<MessagesDto> EditApplicationTask(ApplicationTask task, int employeeId, string employeeName);
        Task<bool> DeleteApplicationTask(int taskid);        
        Task<MessagesDto> CreateNewTaskAndMsgs(ApplicationTask task, int LoggedInEmployeeId);
        Task<TaskItem> CreateNewTaskItem(TaskItem taskItem);
        Task<TaskItem> EditTaskItem(TaskItem taskItem);
        Task<bool> DeleteTaskItem(int taskitemid);
        
    }
}