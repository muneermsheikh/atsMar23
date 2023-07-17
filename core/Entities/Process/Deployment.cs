using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.HR;

namespace core.Entities.Process
{
    public class Deployment: BaseEntity
    {
        public Deployment()
          {
          }

 		public Deployment(int cvRefId, DateTime transactionDate, EnumDeployStatus sequence, 
            EnumDeployStatus nextSequence, DateTime nextEstimatedStageDate) 
		{
			DeployCVRefId = cvRefId;
            TransactionDate = transactionDate;
            Sequence = sequence;
            NextSequence = nextSequence;
			NextStageDate = nextEstimatedStageDate;
		
		}

		public int DeployCVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public EnumDeployStatus Sequence { get; set; }
        public EnumDeployStatus NextSequence { get; set; }
        [ForeignKey("DeployCVRefId")]
        public DateTime NextStageDate { get; set; }
        public CVRef CVRef {get; set;}
    }
}