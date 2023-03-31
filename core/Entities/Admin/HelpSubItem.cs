using System.Collections.Generic;

namespace core.Entities.Admin
{
	public class HelpSubItem: BaseEntity
    {
        public int HelpId { get; set; }
        public int Sequence { get; set; }
        public string HelpText { get; set; }
    }
}