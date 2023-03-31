using core.Dtos;
using core.Entities.Admin;
using core.Params;

namespace core.Interfaces
{
     public interface IEmployeeService
    {
         Task<bool> EditEmployee(Employee employee);
         Task<bool> DeleteEmployee(Employee employee);
         Task<ICollection<Employee>> AddNewEmployees(ICollection<EmployeeToAddDto> employees);
         Task<Pagination<EmployeeBriefDto>> GetEmployeePaginated(EmployeeSpecParams empParams);
         Task<EmployeeDto> GetEmployeeFromIdAsync(int employeeId);
         Task<Employee> GetEmployeeById(int id);
         Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs();
         Task<int> GetEmployeeIdFromAppUserIdAsync(string appUserId);
         Task<EmployeeDto> GetEmployeeBriefAsyncFromAppUserId(string appUserId);
        Task<EmployeeDto> GetEmployeeBriefAsyncFromEmployeeId(int id);
        Task<string> GetEmployeeNameFromEmployeeId(int id);
        Task<ICollection<EmployeePosition>> GetEmployeePositions();
        Task<int> GetEmployeeIdFromEmail(string email);
    }
}