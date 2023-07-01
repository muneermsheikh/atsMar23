export interface IDeploymentDto
{
	id: number;
	cVRefId: number;
	transactionDate: Date;
	sequence: number;
	//stageName: string;
	nextSequence: number;
	//nextStageName: string;
	nextStageDate: Date;
}

export class DeploymentDto implements IDeploymentDto
{
	id=0;
	cVRefId = 0;
	transactionDate = new Date();
	sequence = 0;
	//stageName = '';
	nextSequence = 0;
	//nextStageName = '';
	nextStageDate = new Date();
}