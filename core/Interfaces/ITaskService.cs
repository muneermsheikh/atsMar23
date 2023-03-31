using core.Dtos;
using core.Entities.Tasks;
using core.Params;

namespace core.Interfaces
{
     public interface ITaskService
    {
        Task<ApplicationTask> GetOrCreateTaskFromParams(TaskParams tparams, int loggedinUserId, string LoggedInUserName);
        
        Task<ApplicationTask> GetTaskByParams(TaskParams taskParams);
        Task<ICollection<ApplicationTask>> GetTasksByParams(TaskParams taskParams);
        Task<Pagination<ApplicationTaskDto>> GetTasksPaginated(TaskParams taskParams, int loggedInUserId);
      
        
        //** TODO ** merge all createtasks into one metod
        Task<ApplicationTask> GetOrCreateTask(ApplicationTask task);
        Task<ApplicationTask> NewDLTaskForHRDept(int orderid, int loggedInEmployeeId);
        
        
        Task<bool> SetApplicationTaskStatus(int ApplicationTaskId, DateTime dateOfStatus, 
               string TaskStatus, string UserName, int AppUserId);
        
        Task<ICollection<OrderAssignmentDto>> GetAssignmentDtoFromOrderId(int OrderId);
    }
}