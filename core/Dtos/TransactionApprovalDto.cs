using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class TransactionApprovalDto
    {
        public int FinanceVoucherId { get; set; }
        public int DebitCoaId { get; set; }
        public long AmountToApprove {get; set;}
        public int ApprovedByEmployeeId {get; set;}
        public DateTime ApprovedOn { get; set; }
        
    }
}