using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{

    public class CVRefParams: ParamPages
    {
        public int? CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int? ApplicationNo { get; set; }
        public int? ProfessionId { get; set; }
        public int? AgentId { get; set; }
        public string CustomerName {get; set;}
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
    }
}