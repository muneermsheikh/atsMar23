using System;

namespace core.Dtos
{
     public class DeployRefDto
    {
        public int Id { get; set; }
        public int CvRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string DeploymentStatusname { get; set; }
    }
    
}