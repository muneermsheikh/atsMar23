using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class PendingDebitApprovalDto
    {
        public PendingDebitApprovalDto()
        {
        }

        public PendingDebitApprovalDto(int voucherEntryId, int voucherNo, DateTime voucherDated, 
            int drAccountId, string drAccountName, long drAmount)
        {
            VoucherEntryId = voucherEntryId;
            VoucherNo = voucherNo;
            VoucherDated = voucherDated;
            DrAccountId = drAccountId;
            DrAccountName = drAccountName;
            DrAmount = drAmount;
        }

        public int VoucherEntryId {get; set;}
        public int VoucherNo {get; set;}
        public DateTime VoucherDated {get; set;}
        public int DrAccountId { get; set; }
        public string DrAccountName { get; set; }
        public long DrAmount {get; set;}
        public bool DrEntryApproved {get; set;}
        public int DrEntryApprovedByEmployeeId {get; set;}
        public DateTime DrEntryApprovedOn {get; set;}
    }
}