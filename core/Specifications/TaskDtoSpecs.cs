using core.Dtos;

namespace core.Specifications
{
     public class TaskDtoSpecs: BaseSpecification<ApplicationTaskDto>
    {
        
        public TaskDtoSpecs(string taskStatus) : base (x => x.TaskStatus.ToLower() == taskStatus)
        {
            AddOrderBy(x => x.OrderId);
            AddOrderBy(x => x.OrderItemId);
            AddOrderBy(x => x.TaskDate);
        }
        public TaskDtoSpecs(int taskownerid, bool includeTaskItems, int pageIndex, int pageSize) : 
            base (x => x.TaskStatus.ToLower() != "completed" && x.TaskStatus.ToLower() != "canceled" )
        {
            ApplyPaging(pageSize * (pageIndex -1), pageSize);
            
            AddOrderBy(x => x.TaskDate);
        }
    }
}