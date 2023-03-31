using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class COAToAddDto
    {
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