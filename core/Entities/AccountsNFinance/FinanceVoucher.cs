using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Attachments;

namespace core.Entities.AccountsNFinance
{
	public class FinanceVoucher: BaseEntity
    {
		public FinanceVoucher()
		{
		}

		public FinanceVoucher(string divn, int voucherno, DateTime voucherDated, int coaid, string accountname, long amount, int employeeId, 
             string narration, ICollection<VoucherEntry> voucherentries)
		{
			CoaId=coaid;
            AccountName=accountname;
            Divn = divn;
            VoucherNo=voucherno;
            Amount= amount;
            EmployeeId=employeeId;
            VoucherDated=voucherDated;
            VoucherEntries=voucherentries;
		}

		[MaxLength(1)]
        public string Divn {get; set;}
        public int CoaId { get; set; }
        public string AccountName { get; set; }
        [MaxLength(10), Required]
        public int VoucherNo { get; set; }
        public DateTime VoucherDated {get; set;}
        public long Amount {get; set;}
        public string Narration {get; set;}
        [Required]
        public int EmployeeId {get; set;}
        public int ReviewedById { get; set; }
        [MaxLength(10)]
        public string ReviewedByName {get; set;}
        public DateTime ReviewedOn { get; set; }
        public bool Approved { get; set; }
        public ICollection<VoucherEntry> VoucherEntries { get; set; }
        public ICollection<VoucherAttachment> VoucherAttachments {get; set;}
    }
}