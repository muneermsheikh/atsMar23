using System;
using System.ComponentModel.DataAnnotations;

namespace core.Dtos
{
    public class DeployToAddDto
    {
        [Required]
        public int DeployCVRefId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public int DeployStatusId { get; set; }
        public int DeployStageId { get; set; }
        public int NextDeployStageId { get; set; }
        public DateTime NextDeployStageEstimatedDate { get; set; }
    }
}