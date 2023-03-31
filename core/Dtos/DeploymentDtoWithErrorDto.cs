using System.Collections.Generic;

namespace core.Dtos
{
	public class DeploymentDtoWithErrorDto
    {
        public ICollection<DeploymentObjDto> DeploymentObjDtos { get; set; }
        public ICollection<string> ErrorStrings {get; set;}
    }
}