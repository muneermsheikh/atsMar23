using core.Dtos;
using core.Entities.Admin;
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
        Task<UserHistory> GetHistoryByResumeId(string ResumeId);
        Task<Pagination<UserHistoryDto>> GetUserHistoryPaginated(UserHistoryParams pParams);
        Task<UserHistory> GetOrAddUserHistoryByParams(UserHistoryParams historyParams);
        //Task<UserHistoryDto> GetOrAddUserHistoryByNamePhone(string callerame, string mobileno);
        Task<UserHistory> GetHistoryFromHistoryId(int historyId);
        Task<ICollection<CategoryRefDto>> GetCategoryRefDetails();
    }
}
