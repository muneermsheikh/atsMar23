using System;
using System.Collections.Generic;
using core.Entities.HR;

namespace core.Dtos
{
    public class SelDecisionsToAddParams
    {
        public ICollection<CreateSelDecision> SelDecisionsToAddDto {get; set;}
        public bool SelectionSMSToCandidates { get; set; }
        public bool SelectionEmailToCandidates {get; set;}
        public bool RejectionEmaiLToCandidates { get; set; }
        public bool RejectionSMSToCandidates { get; set; }
        public bool AdvisesToClients { get; set; }
     
    }

}

    