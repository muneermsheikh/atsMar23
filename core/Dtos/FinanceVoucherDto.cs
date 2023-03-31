using System;
using System.Collections.Generic;
using core.Entities.AccountsNFinance;

namespace core.Dtos
{
	public class FinanceVoucherDto
	{
		public FinanceVoucherDto()
		{
		}

		public FinanceVoucherDto(int id, int voucherno,  DateTime voucherDated, int coaid, string accountname, long amount, string narration, 
			string loggedInName, bool approved)
		{
			Id=id;
			CoaId=coaid;
			VoucherDated = voucherDated;
			Amount = amount;
			Narration = narration;
			LoggedInName = loggedInName;
			Approved = approved;
			VoucherNo = voucherno;
			AccountName = accountname;
		}

		public int Id {get; set;}
		public string Divn {get; set;}
		public int VoucherNo {get; set;}
		public DateTime VoucherDated { get; set; }
		public int CoaId {get; set;}
		public string AccountName {get; set;}
		public long Amount { get; set; }
		public string Narration { get; set; }
		public bool Approved {get; set;}
		public string LoggedInName { get; set; }

	}
}