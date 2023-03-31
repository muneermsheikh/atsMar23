using core.Dtos;
using core.Entities.Attachments;
using core.Entities.HR;
using core.Entities.Users;
using core.Params;

namespace core.Interfaces
{
     public interface IUserGetAndUpdateService
    {
        Task<ICollection<UserAttachment>> UpdateUserAttachments(ICollection<UserAttachment> model);
        Task<CandidateWithNewAttachmentDto> UpdateCandidateAsync(Candidate candidate);
        Task<string> DeleteUploadedFile(FileUpload fileupload);
        Task<string> SaveUploadedFiles(ICollection<FileUpload> filesuploaded);

    //gets
        Task<Pagination<Candidate>> GetCandidates(CandidateSpecParams candidateParams);
        Task<Pagination<CandidateBriefDto>> GetCandidateBriefPaginated(CandidateSpecParams prm);
        Task<ICollection<CandidateBriefDto>> GetCandidateListBrief(CandidateSpecParams prm);
        Task<Candidate> GetCandidateByIdWithAllIncludes(int id);
        Task<CandidateBriefDto> GetCandidateByAppNo(int appno);
        Task<CandidateBriefDto> GetCandidateBriefById(int candiadteid);
        Task<ICollection<Candidate>> GetCandidatesWithProfessions(CandidateSpecParams param);
        
    }
}