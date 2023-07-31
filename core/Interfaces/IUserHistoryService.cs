using core.Dtos;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Params;

namespace core.Interfaces
{
     public interface IUserHistoryService
    {
        Task<UserHistory> AddUserContact(UserHistory userHistory);
        Task<bool> DeleteUserContact(UserHistory userHistory);
        Task<bool> DeleteUserContactById(int userContactId);
        Task<ICollection<ContactResult>> GetContactResults();
        Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, LoggedInUserDto loggedInUserDto);
        Task<bool> EditContactHistoryItems(ICollection<UserHistoryItem> items, int LoggedinEmpId);
        Task<bool> DeleteUserHistoryItem(int historyitemid);
        Task<UserHistory> GetHistoryByResumeId(string ResumeId);
        Task<Pagination<UserHistoryDto>> GetUserHistoryPaginated(UserHistoryParams pParams);
        Task<UserHistoryDto> GetOrAddUserHistoryByParams(UserHistoryParams historyParams, string userName);
        //Task<UserHistoryDto> GetOrAddUserHistoryByNamePhone(string callerame, string mobileno);
        Task<UserHistory> GetHistoryWithItemsFromHistoryId(int historyId);
        Task<ICollection<CategoryRefDto>> GetCategoryRefDetails();
        Task<UserHistoryItem>UpdateHistoryItem(UserHistoryItem userhistoryitem, string UserDisplayName);
    }
}
