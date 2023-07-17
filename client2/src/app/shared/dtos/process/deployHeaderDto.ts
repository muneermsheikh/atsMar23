import { IDeployStage } from "../../models/masters/deployStage";
import { Deployment, IDeployment } from "../../models/process/deployment";

export interface IDeployHeaderDto
{
    id: number;
    cvRefId: number;
    applicationNo: number;
    candidateName: string;
    companyName: string;
    categoryRef: string;
    selectedAs: string;
    referredOn: Date;
    selectedOn: Date;
    deployStatuses: IDeployStage[];
    deploy: IDeployment;
}

export class DeployHeaderDto implements IDeployHeaderDto
{
    id = 0
    cvRefId = 0;
    applicationNo= 0;
    candidateName= '';
    companyName= '';
    categoryRef= '';
    selectedAs= '';
    referredOn = new Date('1900-01-01');
    selectedOn = new Date('1900-01-01');
    deployStatuses: IDeployStage[] = [];
    deploy: IDeployment = new Deployment();
}