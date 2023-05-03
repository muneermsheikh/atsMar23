import { DATE } from "ngx-bootstrap/chronos/units/constants";
import { ICVRefDto } from "../cvRefDto";

export interface IDeployment
{
	id: number;
	cVRefId: number;
	transactionDate: Date;
	stageId: number;
	nextStageId: number;
	nextEstimatedStageDate: Date;
}


export class Deployment implements IDeployment
{
	id: number=0;
	cVRefId: number=0;
	transactionDate: Date=new Date();
	stageId: number=0;
	nextStageId: number=0;
	nextEstimatedStageDate: Date=new Date();
}