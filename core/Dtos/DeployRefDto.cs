using System;

namespace core.Dtos
{
     public class DeployRefDto
    {
        public DeployRefDto(int id, int cvRefId, DateTime transactionDate, int sequence, string deploymentStatusname)
        {
            Id = id;
            DeployCVRefId = cvRefId;
            TransactionDate = transactionDate;
            DeploymentStatusname = deploymentStatusname;
            Sequence = sequence;
        }

        public int Id { get; set; }
        public int DeployCVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Sequence {get; set;}
        public string DeploymentStatusname { get; set; }
    }
    
}