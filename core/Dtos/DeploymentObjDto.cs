using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class DeploymentObjDto
    {
        public int Id { get; set; }
        public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int StageId {get; set;}
        public string StageName { get; set; }
        public string NextStageName { get; set; }
        public int NextStageId {get; set;}
        public DateTime NextEstimatedStageDate { get; set; }
    }
}