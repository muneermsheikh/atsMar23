using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class Help: BaseEntity
    {
        public string Topic { get; set; }
        public ICollection<HelpItem> HelpItems {get; set;}
    }
}