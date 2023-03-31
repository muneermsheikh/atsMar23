using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class AutoAdvise: BaseEntity
    {
        public string BaseClassTableName {get; set;}
        public string BaseClassFieldName {get; set;}
        public int EmployeeId {get; set;}

    }
}