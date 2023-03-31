using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class VoucherToAddNewPayment
    {
        public int DebitCOAId { get; set; }
        public string DebitAccountName {get; set;}
        public int CreditCOAId {get; set;}
        public string CreditAccountName {get; set;}
        public long Amount {get; set;}
        public DateTime PaymentDate {get; set;}
        public string Narration {get; set;}
        public bool DrEntryRequiresApproval {get; set;}
    }
}