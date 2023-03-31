using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class StatementOfAccountItemDto
    {
        public int VoucherNo { get; set; }
        public DateTime TransDate { get; set; }
        public int CoaId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
	    public long Cr {get; set;}
        public string Narration { get; set; }
    }

    
}

