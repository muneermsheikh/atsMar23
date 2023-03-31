using core.Dtos;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Users;
using core.Params;

namespace core.Interfaces
{
     public interface IUserService
    {
        //creates or adds
        
        //Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, ICollection<IFormFile> UserFormFiles, int loggedInEmployeeId);
        Task<Candidate> CreateCandidateObject(RegisterDto registerDto, int loggedInEmployeeId);
        Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, int loggedInEmployeeId);
        Task<Employee> CreateEmployeeAsync(RegisterEmployeeDto registerDto);
        Task<CustomerOfficial> CreateCustomerOfficialAsync(RegisterDto registerDto);
        Task<bool> AddUserAttachments(ICollection<UserAttachment> userattachments);
        Task<bool> UpdateCandidatePhoto(int CandidateId, string photoUrl);
        
        //Task<Candidate> GetCandidateByIdAsync(int id);
        
        
        //Task<Candidate> GetCandidateBySpecsUserIdAsync(int userId);
        //Task<Candidate> GetCandidateBySpecsIdentityIdAsync(int identityUserId);
        Task<ICollection<UserProfession>> EditUserProfessions(UserAndProfessions userProfessions);
        Task<ICollection<CandidateCity>> GetCandidateCityNames();
        Task<string> CheckPPNumberExists(string ppNumber);
        Task<bool> CheckAadharNoExists(string aadharNo);
        Task<bool> CheckCandidateAadharNoExists(string aadharNo);
        
        Task<string> GetFileUrl(int attachmentid);

            //masters
        Task<string> GetCategoryNameFromCategoryId(int id);
        Task<string> GetCustomerNameFromCustomerId(int id);

        Task<Pagination<UserDto>> GetAppUsersPaginated(UserParams userparams);
    }
}