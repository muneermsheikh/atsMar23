using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.AccountsNFinance
{
	public class VoucherEntry : BaseEntity
	{
	public VoucherEntry()
	{
	}

	public int FinanceVoucherId { get; set; }
	public DateTime TransDate { get; set; }
	public int CoaId { get; set; }
	public string AccountName { get; set; }
	public long Dr {get; set;}
	public long Cr {get; set;}
	public string Narration { get; set; }
	public int DrEntryApprovedByEmployeeById { get; set; }
	[MaxLength(10)]
	public DateTime DrEntryApprovedOn { get; set; }
	public bool DrEntryApproved { get; set; }
    
	}
}