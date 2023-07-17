using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.AccountsNFinance;

namespace core.Dtos.fiance
{
    public class CoaDto
    {
		public int Id {get; set;}
        public string Divn { get; set; }
		public string AccountType { get; set; }
		public string AccountName { get; set; }
		public string AccountClass {get; set;}
		public long ClBalance { get; set; }

    }
}