using System;

namespace core.Dtos
{
    public class RejDecisionToAddDto
    {
        public int CVRefId { get; set; }
        public DateTime DecisionDate { get; set; }
        public int SelectionStatusId { get; set; }
        public string Remarks { get; set; }
     }
}