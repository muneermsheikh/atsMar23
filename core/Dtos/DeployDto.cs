using core.Entities.Process;

namespace core.Dtos
{
    public class DeployDto
    {
        public DeployDto(int id, int cVRefId, DateTime transactionDate, int sequence, int nextSequence, DateTime nextStageDate)
        {
            Id = id;
            DeployCVRefId = cVRefId;
            TransactionDate = transactionDate;
            Sequence = sequence;
            NextSequence = nextSequence;
            NextStageDate = nextStageDate;
        }

        public int Id { get; set; }
        public int DeployCVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        //public EnumDeployStatus Sequence { get; set; }
        public int Sequence { get; set; }
        //public EnumDeployStatus NextSequence { get; set; }
        public int NextSequence { get; set; }

        public DateTime NextStageDate { get; set; }
    }
}