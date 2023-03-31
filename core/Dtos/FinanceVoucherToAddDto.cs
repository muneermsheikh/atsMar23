using System;
using System.Collections.Generic;
using core.Entities.AccountsNFinance;

namespace core.Dtos
{
	public class FinanceVoucherToAddDto
	{
		public FinanceVoucherToAddDto()
		{
		}

		public FinanceVoucherToAddDto(int id, string divn, int voucherNo, DateTime voucherDated, int coaid, long amount, string narration, 
			ICollection<VoucherEntry> voucherEntries, string loggedInName)
		{
			Id=id;
			Divn=divn;
			VoucherNo=voucherNo;
			VoucherDated=voucherDated;
			CoaId=coaid;
			Amount = amount;
			Narration = narration;
			LoggedInName = loggedInName;
			VoucherEntries = voucherEntries;
		}

		public int Id {get; set;}
		public string Divn {get; set;}
		public int VoucherNo {get; set;}
		public DateTime VoucherDated {get; set;}
		public int CoaId {get; set;}
		public long Amount { get; set; }
		public string Narration { get; set; }
		public string LoggedInName { get; set; }
		public ICollection<VoucherEntry> VoucherEntries {get; set;}
	}
}