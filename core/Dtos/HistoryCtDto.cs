using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class HistoryCtDto
    {
        public int TotalCount { get; set; }
        public int TotalPositives { get; set; }
        public int TotalNegatives { get; set; }
        public int TotalContacted { get; set; }

    }
}