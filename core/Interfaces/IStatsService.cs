using core.Dtos;
using core.Params;

namespace core.Interfaces
{
     public interface IStatsService
    {
         Task<ICollection<OpeningsDto>> GetCurrentOpenings (StatsParams param);
         //Task<OrderItemTransDto> GetOrderItemTransactions (StatsTransParams param);
         Task<Pagination<EmployeePerformanceDto>> GetEmployeePerformance (EmployeePerfParams empPerf);
    }
}