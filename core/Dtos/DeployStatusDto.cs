using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class DeployStatusDto
    {
        public int StageId { get; set; }
        public string StatusName { get; set; }
    }
}