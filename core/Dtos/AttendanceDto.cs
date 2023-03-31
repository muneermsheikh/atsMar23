using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class AttendanceDto
    {
        public int OrderId { get; set; }
        public bool IsAssigned {get; set;}
        public bool IsAttending {get; set;}
        public bool HasDeclined {get; set;}
        public bool IsSelected {get; set;}
        public bool IsRejected {get; set;}
    }
}