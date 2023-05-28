using core.Dtos.Admin;
using core.Entities.Admin;

namespace core.Interfaces
{
     public interface IDLForwardService
    {
        Task<string> ForwardDLToAgents(DLForwardToAgent dlforward, int LoggedInEmpId, string LoggedInEmpKnownAs, string LoggedInEmpEmail);
        Task<ICollection<DLForwardToAgent>> DLForwardsForActiveDLs();
        Task<DLForwardToAgent> DLForwardsForDL(int orderid);
        Task<DLForwardedDateDto> OrderItemForwardedToStats (int OrderId);

    }
}