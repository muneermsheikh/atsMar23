using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class InterviewItemDto
    {
        public int Id {get; set;}
        public int InterviewId { get; set; }
        public int OrderItemId { get; set; }
        public bool Checked {get; set;}
        public int SrNo {get; set;}
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto {get; set;}
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewStatus {get; set;}
        public string ConcludingRemarks {get; set;}
        
    }
}