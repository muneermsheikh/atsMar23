using core.Dtos;
using core.Entities.HR;
using core.Params;

namespace core.Interfaces
{
     public interface IEmploymentService
    {
         Task<Employment> AddEmployment (Employment employment);
         Task<bool> EditEmployment (Employment employment);
         Task<bool> DeleteEmployment (int employmentId);
            Task<Pagination<Employment>> GetEmployments(EmploymentParams empParams);
         /*
         Task<Employment> GetEmployment(int CVRefId);
         Task<Employment> GetEmploymentFromSelId(int Id);
        Task<ICollection<EmploymentDto>> GetEmploymentDtoFromOrderNo (int orderNo);
        Task<ICollection<EmploymentDto>> GetEmploymentDtoBetwenDates (DateTime fromDate, DateTime uptoDate);
        Task<ICollection<EmploymentDto>> GetEmploymentDtoFromCVRefId (int cvrefid);
        */
         
    }
}