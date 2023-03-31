using core.Dtos;
using core.Params;

namespace core.Interfaces
{
     public interface IProspectiveCandidateService
    {
        Task<Pagination<ProspectiveCandidateEditDto>> GetProspectiveCandidates(ProspectiveCandidateParams pParams);
        Task<UserDto> ConvertProspectiveToCandidate(ProspectiveCandidateAddDto dto);
        Task<Pagination<UserHistoryHeaderDto>> GetCallRecordHeaders(UserHistoryHeaderParams hParams);
        //Task<Pagination<UserHistoryHeaderDto>> GetCallRecordHeadersWithConstraints(UserHistoryHeaderParams hParams);
        //Task<string> ReadAndSaveToDbExcelFile(string filePathName, int userid, string username);
        Task<string> EditProspectiveCandidates(ICollection<ProspectiveUpdateDto> prospectives, LoggedInUserDto UserDto);
        //Task<Pagination<ProspectiveCandidateHeader>> GetProspectiveCandidateHeaderPages(ProspectiveCandidateParams pParams);
        Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams pParams);
    }
}