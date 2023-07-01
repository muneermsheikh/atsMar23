using System;
using System.Collections.Generic;
using core.Entities.HR;

namespace core.Dtos
{
    public class ChecklistHRDto
    {
        public ChecklistHRDto()
        {
        }

        public ChecklistHRDto(int id, int candidateId, int applicationNo, string candidateName, 
            int orderItemId, string categoryRef, string orderRef, int userLoggedId, 
            string userLoggedName, int charges, ICollection<ChecklistHRItem> checklistHRItems)
        {
            Id = id;
            CandidateId = candidateId;
            ApplicationNo = applicationNo;
            CandidateName = candidateName;
            OrderItemId = orderItemId;
            CategoryRef = categoryRef;
            OrderRef = orderRef;
            UserLoggedId = userLoggedId;
            UserLoggedName = userLoggedName;
            Charges = charges;
            ChecklistHRItems = checklistHRItems;
        }

        public int Id {get; set;}
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public int OrderItemId { get; set; }
        public string CategoryRef {get; set;}
        public string OrderRef { get; set; }
        public int UserLoggedId { get; set; }
        public string UserLoggedName {get; set;}
        public DateTime CheckedOn {get; set;}
        public int Charges {get; set;}
        public int ChargesAgreed {get; set;}
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}

        public String HrExecComments {get; set;}
        public bool ChecklistedOk {get; set;}
        public bool AssessmentIsNull {get; set;}
        public bool RequireInternalReview {get; set;}
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
    }
}