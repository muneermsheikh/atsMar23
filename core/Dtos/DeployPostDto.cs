using System;
using core.Entities.Process;

namespace core.Dtos
{
    public class DeployPostDto
    {
        public int DeployCVRefId { get; set; }
        public EnumDeployStatus StageId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}