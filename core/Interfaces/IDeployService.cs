using core.Dtos;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Process;
using core.Params;

namespace core.Interfaces
{
     public interface IDeployService
    {
        Task<Pagination<DeploymentPendingDto>> GetPendingDeployments(DeployParams depParams);
        Task<int> CountOfPendingDeployments();
        Task<CVRef> GetDeploymentsById(int cvrefid);
        Task<CVReferredDto> GetDeploymentDto(int cvrefid);
        Task<ICollection<DeploymentDto>> GetDeployments(int cvrefid);
        Task<CVRef> GetDeploymentsByCandidateAndOrderItem(int candidateId, int orderItemId);
        Task<DeploymentDtoWithErrorDto> AddDeploymentTransactions(ICollection<Deployment> deployPostsDto, int loggedInEmployeeId);
        Task<bool> EditDeploymentTransactions (ICollection<Deployment> deps);
        Task<bool> EditDeploymentTransaction (DeploymentDto deploy);
        Task<bool> DeleteDeploymentTransactions (int deployid);
        Task<ICollection<DeployStage>> GetDeployStatuses();
        
    }
}