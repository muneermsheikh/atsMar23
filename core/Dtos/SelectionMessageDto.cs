using core.Entities.HR;

namespace core.Dtos
{
	public class SelectionMessageDto
    {
		public SelectionMessageDto(string customerName, string customerCity, int orderNo, string categoryName, Employment employmt, 
			SelectionDecision selDecision, int applicationNo, int candidateId, string candidateTitle, string candidateName, string candidateGender, 
			string candidateEmail, string candidateKnownAs, int hRExecId, string hRExecName, string hRExecEmail, int hRupId, 
			string hRSupName, string hRSupEmail)
		{
			CustomerName = customerName;
			CustomerCity = customerCity;
			OrderNo = orderNo;
			CategoryName = categoryName;
			employment = employmt;
			selectionDecision = selDecision;
			ApplicationNo = applicationNo;
			CandidateId = candidateId;
			CandidateTitle = candidateTitle;
			CandidateName = candidateName;
			CandidateGender = candidateGender;
			CandidateEmail = candidateEmail;
			CandidateKnownAs = candidateKnownAs;
			HRExecId = hRExecId;
			HRExecName = hRExecName;
			HRExecEmail = hRExecEmail;
			HRupId = hRupId;
			HRSupName = hRSupName;
			HRSupEmail = hRSupEmail;
		}

		public string CustomerName {get; set;}
		public string CustomerCity {get; set;}
		public int OrderNo { get; set; }
		public string CategoryName { get; set; }
		public string RejectionReason {get; set;}
		public Employment employment {get; set;}
		public SelectionDecision selectionDecision {get; set;}
		public int ApplicationNo { get; set; }
		public int CandidateId { get; set; }
		public string CandidateTitle {get; set;}
		public string CandidateName {get; set;}
		public string CandidateGender { get; set; }
		public string CandidateEmail { get; set; }
		public string CandidateKnownAs {get; set;}
		public int HRExecId {get; set;}
		public string HRExecName { get; set; }
		public string HRExecEmail {get; set;}
		public int HRupId {get; set;}
		public string HRSupName { get; set; }
		public string HRSupEmail {get; set;}
    }
}
                