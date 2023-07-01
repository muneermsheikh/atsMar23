import { IDeploymentDto } from "../../models/process/deploymentdto";

export interface IDeploymentDtoWithErrorDto
{
	deploymentObjDtos: IDeploymentDto[];
	errorStrings: string[];
}