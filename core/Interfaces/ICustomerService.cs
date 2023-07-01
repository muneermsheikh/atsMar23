using core.Dtos;
using core.Entities;
using core.Specifications;

namespace core.Interfaces
{
     public interface ICustomerService
    {
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        Task<CustomerDto> AddCustomer (RegisterCustomerDto dto);
        Task<ICollection<CustomerDto>> AddCustomers (ICollection<RegisterCustomerDto> dtos);
        
        Task<bool> EditCustomer(Customer customer);
        Task<ICollection<CustomerDto>> GetCustomersAsync(string userType);
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<CustomerBriefDto> GetCustomerBriefById(int id);
        Task<Pagination<CustomerDto>> GetCustomersPaginatedAsync(CustomerParams custParam);
        Task<Pagination<CustomerBriefDto>> GetCustomersBriefAsync(CustomerSpecParams custParam);

        Task<CustomerDto> GetCustomerByUserNameAsync(string username);
        Task<string> GetCustomerNameFromId (int Id);
        Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndName (string customerType);
        Task<ICollection<ClientIdAndNameDto>> GetClientIdAndName();
        Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndNames(ICollection<int> customerIds);
        Task<bool> CustomerExistsByIdAsync(int id);
        Task<ICollection<CustomerCity>> GetCustomerCityNames (string customerType);
        Task<ICollection<string>> GetCustomerIndustryTypes(string customerType);
        Task<ICollection<CustomerOfficialDto>> GetOfficialDetails ();
        Task<CustomerOfficialDto> GetCustomerOfficialDetail (int CustomerOfficialId);
    }
}