using System.Collections.Generic;


namespace core.Entities.Admin
{
	public class HelpItem: BaseEntity
    {
        public int HelpId {get; set;}
        public int Sequence {get; set;}
        public string HelpText {get; set;}
        public ICollection<HelpSubItem> HelpSubItems {get; set;}
    }
}