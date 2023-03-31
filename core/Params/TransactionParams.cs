using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{
    public class TransactionParams: ParamPages
    {
		public TransactionParams()
		{
		}

		public string Divn {get; set;}
		public string VoucherNo {get; set;}
		public DateTime VoucherDated {get; set;}
		public int CoaId {get; set;}
		public string AccountName {get; set;}
		public DateTime DateFrom {get; set;}
		public DateTime DateUpto {get; set;}
		public long Amount { get; set; }
		public int EmployeeId { get; set; }
		public string Narration {get; set;}
    	}
}