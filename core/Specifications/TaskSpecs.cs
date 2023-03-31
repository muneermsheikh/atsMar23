using core.Entities.Tasks;
using core.Params;

namespace core.Specifications
{
     public class TaskSpecs: BaseSpecification<ApplicationTask>
    {
        public TaskSpecs(TaskParams eParams)
            : base(x => 
                (!eParams.ApplicationNo.HasValue || x.ApplicationNo == eParams.ApplicationNo) &&
                (!eParams.OrderNo.HasValue || x.OrderNo == eParams.OrderNo) &&
                (!eParams.OrderItemId.HasValue || x.OrderItemId == eParams.OrderItemId) &&
                (!eParams.CandidateId.HasValue || x.CandidateId == eParams.CandidateId) &&
                (!eParams.OrderId.HasValue || x.OrderId == eParams.OrderId) &&
                (!eParams.DateFrom.HasValue && !eParams.DateUpto.HasValue || 
                    Nullable.Compare(x.TaskDate.Date, eParams.DateFrom) >= 0 && 
                    Nullable.Compare(x.TaskDate.Date, eParams.DateUpto) >= 0) &&
                (!eParams.DateFrom.HasValue && eParams.DateUpto.HasValue || 
                    DateTime.Compare(x.TaskDate.Date, (DateTime)eParams.DateFrom) == 0) &&
                (!eParams.CompleteBy.HasValue || 
                    DateTime.Compare(Convert.ToDateTime(x.CompleteBy).Date, (DateTime)eParams.CompleteBy) <= 0) &&
                (string.IsNullOrEmpty(eParams.TaskStatus) || x.TaskStatus.ToLower() == eParams.TaskStatus.ToLower()) &&
                (eParams.TaskOwnerId > 0 || x.TaskOwnerId == eParams.TaskOwnerId) &&
                (eParams.AssignedToId > 0  || x.AssignedToId == eParams.AssignedToId) &&
                (!eParams.TaskTypeId.HasValue || x.TaskTypeId == eParams.TaskTypeId) 
                /* &&
                (!eParams.CandidateId.HasValue || x.PersonType.ToLower() == eParams.PersonType.ToLower() && x.CandidateId == eParams.CandidateId) 
                */
            )
        {
            if (eParams.IncludeItems) AddInclude(x => x.TaskItems);
            ApplyPaging(eParams.PageSize * (eParams.PageIndex -1), eParams.PageSize);
            switch(eParams.Sort.ToLower())
            {
                case "taskdate":
                    AddOrderBy(x => x.TaskDate);
                    break;
                case "taskdatedesc":
                    AddOrderByDescending(x => x.TaskDate);
                    break;
                case "taskstatus":
                    AddOrderBy(x => x.TaskStatus);
                    break;
                case "assignedto":
                    AddOrderBy(x => x.AssignedToName);
                    break;
                case "assignedtodesc":
                    AddOrderByDescending(x => x.AssignedToName);
                    break;
                case "orderno":
                    AddOrderBy(x => x.OrderNo);
                    break;
                case "ordernodesc":
                    AddOrderByDescending(x => x.OrderNo);
                    break;
                default:
                    break;
            }


        }
        
        public TaskSpecs(string taskStatus) : base (x => x.TaskStatus.ToLower() == taskStatus)
        {
            AddOrderBy(x => x.OrderId);
            AddOrderBy(x => x.OrderItemId);
            AddOrderBy(x => x.TaskDate);
        }
        public TaskSpecs(int taskownerid, bool includeTaskItems, int pageIndex, int pageSize) : 
            base (x => x.TaskStatus.ToLower() != "completed" && x.TaskStatus.ToLower() != "canceled" )
        {
            ApplyPaging(pageSize * (pageIndex -1), pageSize);
            if (includeTaskItems) AddInclude(x => x.TaskItems);
            AddOrderBy(x => x.TaskDate);
        }
    }
}