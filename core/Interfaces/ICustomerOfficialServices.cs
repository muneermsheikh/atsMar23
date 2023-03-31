using core.Dtos;

namespace core.Interfaces
{
     public interface ICustomerOfficialServices
    {
        Task<CustomerOfficialDto> GetCustomerOfficialDetail(int CustomerOfficialId);
    }
}