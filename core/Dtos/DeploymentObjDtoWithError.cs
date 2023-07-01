using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class DeploymentObjDtoWithError
    {
        public ICollection<DeploymentDto> DeploymentObjDtos {get; set;}
        public ICollection<string> ErrorStrings{get; set;}
    }
}