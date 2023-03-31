using System.ComponentModel.DataAnnotations;

namespace core.Entities.Process
{
    public class DeployStatus: BaseEntity
    {
        [Required]
        public int StageId { get; set; }
        [Required]
        public string StatusName { get; set; }
        [Required]
        public string ProcessName { get; set; }
        [Required]
        public int NextStageId {get; set;}
        public int WorkingDaysReqdForNextStage { get; set; }
    }
}