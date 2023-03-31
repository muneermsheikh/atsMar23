using core.Dtos;
using core.Entities.HR;
using core.Entities.Process;
using core.Params;

namespace core.Interfaces
{
     public interface IDeployService
    {
        Task<Pagination<CVRefAndDeployDto>> GetPendingDeployments(DeployParams depParams);
        Task<int> CountOfPendingDeployments();
        Task<CVRef> GetDeploymentsById(int cvrefid);
        Task<CVReferredDto> GetDeploymentDto(int cvrefid);
        Task<ICollection<DeploymentObjDto>> GetDeploymentsObject(int cvrefid);
        Task<CVRef> GetDeploymentsByCandidateAndOrderItem(int candidateId, int orderItemId);
        Task<DeploymentDtoWithErrorDto> AddDeploymentTransactions(ICollection<Deploy> deployPostsDto, int loggedInEmployeeId);
        Task<bool> EditDeploymentTransactions (ICollection<Deploy> deploys);
        Task<bool> DeleteDeploymentTransactions (int deployid);
        Task<ICollection<DeployStatusDto>> GetDeployStatuses ();
        
    }
}