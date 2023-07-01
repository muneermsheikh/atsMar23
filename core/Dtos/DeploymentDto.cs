namespace core.Dtos
{
    public class DeploymentDto
    {
        public int Id { get; set; }
        public int CVRefId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Sequence {get; set;}
        //public string StageName { get; set; }
        //public string NextStageName { get; set; }
        public int NextSequence {get; set;}
        public DateTime NextStageDate { get; set; }
    }
}