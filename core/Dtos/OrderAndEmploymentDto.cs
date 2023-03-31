using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.HR;

namespace core.Dtos
{
    public class OrderForSelectionDto
    {
        public int OrderNo { get; set; }
        public string CompanyName { get; set; }

        public Employment employment {get; set;}
    }

    
    
}