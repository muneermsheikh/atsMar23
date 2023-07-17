
export interface IDeploymentPendingDto {
    id: number;
    checked: boolean;
    deployCVRefId: number;
    referredOn: Date;
    selectedOn: Date;
    applicationNo: number;
    candidateName: string;

    customerName: string;
    orderNo: number;
    orderDate: Date;
    categoryName: string;
    selDecisionDate: Date;
    deploySequence: number;
    deployStageDate: Date;
    nextSequence: number;
    nextStageDate: Date;
}

export class DeploymentPendingDto implements IDeploymentPendingDto {
    id: number=0;
    checked: boolean=false;
    deployCVRefId: number=0;
    referredOn = new Date('1900-01-01');
    selectedOn = new Date('1900-01-01');
    applicationNo: number=0;
    candidateName: string='';
    
    customerName: string='';
    orderNo: number=0;
    orderDate: Date=new Date();
    categoryName: string='';
    
    selDecisionDate: Date=new Date();
    deploySequence: number=0;
    deployStageDate = new Date('1900-01-01');
    nextSequence: number=0;
    nextStageDate: Date=new Date();

}