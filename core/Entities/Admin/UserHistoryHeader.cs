using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    //headers of the calling task for executives
    public class UserHistoryHeader: BaseEntity
    {
        public string CategoryRefCode { get; set; }
        public string CategoryRefName {get; set;}
        public string CustomerName { get; set; }
        public DateTime CompleteBy {get; set;}
        public int AssignedToId {get; set;}
        public string AssignedToName { get; set; }
        public int AssignedById {get; set;}
        public string AssignedByName { get; set; }
        public DateTime AssignedOn {get; set;}
        public string Status {get; set;}
        public bool Concluded { get; set; }
        public ICollection<UserHistory> UserHistories {get; set;}
    }
}