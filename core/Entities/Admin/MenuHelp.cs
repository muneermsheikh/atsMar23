using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class MenuHelp: BaseEntity
    {
        public string Menu { get; set; }
        public string MenuUrl {get; set;}
        public string Explanation {get; set;}
        public ICollection<MenuItemHelp> MenuItems {get; set;}
    }

    public class MenuItemHelp: BaseEntity
    {
        public string SubMenu {get; set;}
        public string MenuUrl {get; set;}
        public string Explanation {get; set;}
        public ICollection<MenuSubItemHelp> SubMenuItems {get; set;}
    }
    public class MenuSubItemHelp: BaseEntity
    {
        public string SubMenuItem {get; set;}
        public string MenuUrl {get; set;}
        public string Explanation {get; set;}
    }
}