using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.AccountsNFinance
{
    public class Coa: BaseEntity
    {
		public Coa()
		{
		}

		public Coa(string divn, string accountType, string accountName, string classname, long opba)
		{
			Divn = divn;
			AccountType = accountType;
			AccountName = accountName;
			AccountClass = classname;
			OpBalance = opba;
		}

		[MaxLength(1), Required]    
		public string Divn { get; set; }
		[MaxLength(1), Required]
		public string AccountType { get; set; }
		[Required, MaxLength(100)]
		public string AccountName { get; set; }
		public string AccountClass {get; set;}
		public long OpBalance { get; set; }
    }
}