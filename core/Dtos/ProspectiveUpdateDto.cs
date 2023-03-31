using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ProspectiveUpdateDto
    {
        public int ProspectiveId { get; set; }
        public string NewStatus { get; set; }
        public bool Closed {get; set;}
        public string Remarks { get; set; }
    }
}