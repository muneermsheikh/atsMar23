using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class StatementOfAccountDto
    {
		public StatementOfAccountDto()
		{
		}

		public StatementOfAccountDto(int accountId, string accountnam, ICollection<StatementOfAccountItemDto> statementOfAccountItems)
		{
			AccountId = accountId;
			AccountName = accountnam;
			StatementOfAccountItems = statementOfAccountItems;
		}

		public int AccountId { get; set; }
        	public string AccountName { get; set; }
		public DateTime FromDate {get; set;}
		public DateTime UptoDate {get; set;}
		public long OpBalance {get; set;}
		public long ClBalance {get; set;}
        	public ICollection<StatementOfAccountItemDto> StatementOfAccountItems { get; set; }
    }
}