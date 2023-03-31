using System;

namespace core.Dtos
{
    public class TaskDashboardDto
    {
        public int ApplicationTaskId { get; set; }
        public int TaskTypeId { get; set; }
        public DateTime TaskDate { get; set; }
        public int TaskOwnerId {get; set;}
        public string TaskownerName { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public string TaskDescription {get; set;}
        public DateTime CompleteBy {get; set;}
        public string TaskStatus {get; set;}

    }
}