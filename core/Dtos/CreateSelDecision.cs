using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class CreateSelDecision
    {
         public int CVRefId { get; set; }
        public DateTime DecisionDate { get; set; }
        public int SelectionStatusId { get; set; }
        public string Remarks { get; set; }
    }
}