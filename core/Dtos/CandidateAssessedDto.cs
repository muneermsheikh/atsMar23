namespace core.Dtos
{
     public class CandidateAssessedDto
    {
        public CandidateAssessedDto()
        {
        }

        public int Id {get; set;}
        public int OrderItemId { get; set; }
        public string CategoryRef {get; set;}
        public string CategoryName {get; set;}
        public bool requireInternalReview {get; set;}
        public string CustomerName {get; set;}
        public int CandidateId {get; set;}
        public string CandidateName {get; set;}
        public int ApplicationNo {get; set;}
        public string AgentName {get; set;}
        public string CityName { get; set; }
        public int AssessedById { get; set; }
        public string AssessedByName {get; set;}
        public string AssessedResult {get; set;}
        public string checklistedByName {get; set;}
        public DateTime ChecklistedOn {get; set;}
        public string Charges {get; set;}
        public int CvRefId {get; set;}
        public DateTime AssessedOn { get; set; }
        public string Remarks { get; set; }

    }
}