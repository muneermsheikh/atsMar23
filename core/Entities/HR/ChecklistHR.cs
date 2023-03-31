using System;
using System.Collections.Generic;
using core.Entities.Orders;
using core.Entities.Users;

//HR Executives have to deal with candidates to identify their suitability for client requirements
//Certain identified aprameters have to be discussed with candidates, and noted for future references
//this model saves the data in the table
namespace core.Entities.HR
{
    public class ChecklistHR: BaseEntity
    {
         public ChecklistHR()
          {
          }

          public ChecklistHR(int candidateId, int orderItemId, int userId, DateTime checkedOn, ICollection<ChecklistHRItem> checklistHRItems)
          {
               CandidateId = candidateId;
               OrderItemId = orderItemId;
               UserId = userId;
               CheckedOn = checkedOn;
               ChecklistHRItems = checklistHRItems;
          }

        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckedOn {get; set;}
        public String HrExecComments {get; set;}
        public int Charges {get; set;}
        public int ChargesAgreed {get; set;}
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}
        public bool ChecklistedOk {get; set;}
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
        public Candidate Candidate {get; set;}
        public OrderItem OrderItem {get; set;}

    }
}