export interface IDeployment
{
	id: number;
	cVRefId: number;
	transactionDate: Date;
	sequence: number;
	nextSequence: number;
	nextStageDate: Date;
}


export class Deployment implements IDeployment
{
	id: number=0;
	cVRefId: number=0;
	transactionDate: Date=new Date();
	sequence: number=0;
	nextSequence: number=0;
	nextStageDate: Date=new Date();
}