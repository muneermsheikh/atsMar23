using System;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.HR;

namespace core.Entities.Process
{
    public class Deploy: BaseEntity
    {
          public Deploy()
          {
          }

          public Deploy(int cVRefId, DateTime transactionDate, EnumDeployStatus sequence)
          {
               CVRefId = cVRefId;
               TransactionDate = transactionDate;
               Sequence = sequence;
          }

		public Deploy(int cvRefId, DateTime transactionDate, EnumDeployStatus sequence, 
            EnumDeployStatus nextSequence, DateTime nextEstimatedStageDate) 
		{
			CVRefId = cvRefId;
            TransactionDate = transactionDate;
            Sequence = sequence;
            NextSequence = nextSequence;
			NextStageDate = nextEstimatedStageDate;
		
		}

		public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public EnumDeployStatus Sequence { get; set; }
        public EnumDeployStatus NextSequence { get; set; }
        [ForeignKey("CVRefId")]
        public DateTime NextStageDate { get; set; }
        public CVRef CVRef {get; set;}
    }
}