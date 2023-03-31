using System;
using core.Dtos;

namespace core.Params
{
    public class TaskParams: ParamPages
    {
        public TaskParams()
        {
        }

        public TaskParams(TaskParams taskParams)
        {
        }

        public TaskParams(int orderItemId, int candidateId, bool includeItems, int pageSize)
        {
            OrderItemId = orderItemId;
            CandidateId = candidateId;
            IncludeItems = includeItems;
            PageSize = pageSize;
        }

        public int TaskId {get; set;}
        public int TaskOwnerId {get; set;}
        public int AssignedToId {get; set;}=0;
        public int? CandidateId {get; set;}
        public string PersonType {get; set;}
        public string ResumeId {get; set;}
        public int? OrderId {get; set;}
        public int? OrderNo { get; set; }
        public int? OrderItemId {get; set;}
        public string TaskDescription {get; set;}
        public DateTime? CompleteBy {get; set;}
        public string TaskStatus {get; set;}
        public string CurrentUserName { get; set; }
        public int? TaskTypeId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateUpto { get; set; }
        public int? ApplicationNo { get; set; }
        public bool IncludeItems { get; set; }=false;
    }
}