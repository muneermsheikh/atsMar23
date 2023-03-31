using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class PendingDebitApprovalDto
    {
        public int VoucherEntryId {get; set;}
        public int VoucherNo {get; set;}
        public DateTime VoucherDated {get; set;}
        public int DrAccountId { get; set; }
        public string DrAccountName { get; set; }
        public int CrAccountId {get; set;}
        public string CrAccountName {get; set;}
        public long CrAmount {get; set;}
        public long DrAmount {get; set;}
        public bool DrEntryReviewed {get; set;}
        public bool DrEntryApproved {get; set;}
        public int DrEntryReviewedByEmployeeId {get; set;}
        public DateTime DrEntryReviewedOn {get; set;}
        public int ConfirmationRequestedByEmployeeId { get; set; }
    }
}