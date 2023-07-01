using System.Collections.Generic;

namespace core.Dtos
{
    public class NewBaseType
    {
        public ICollection<DeploymentDto> DeploymentObjDtos { get; set; }
    }

    public class DeploymentDtoWithErrorDto : NewBaseType
    {
        public ICollection<string> ErrorStrings {get; set;}
    }
}