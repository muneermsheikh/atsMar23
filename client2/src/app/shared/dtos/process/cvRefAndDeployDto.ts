
export interface ICVRefAndDeployDto {
    id: number;
    checked: boolean;
    cvRefId: number;
    customerName: string;
    orderId: number;
    orderNo: number;
    orderDate: Date;
    orderItemId: number;
    categoryName: string;
    categoryRef: string;
    customerId: number;
    candidateId: number;

    applicationNo: number;
    candidateName: string;
    referredOn: Date;
    selectedOn: Date;
    refStatus: number;
    deployStageId: number;
    deployStageName: string;
    nextStageId: number;
    nextStageDate: Date;
    transactionDate: Date;
}


export class CVRefAndDeployDto implements ICVRefAndDeployDto {
    id: number=0;
    checked: boolean=false;
    cvRefId: number=0;
    customerName: string='';
    orderId: number=0;
    orderNo: number=0;
    orderDate: Date=new Date();
    orderItemId: number=0;
    categoryName: string='';
    categoryRef: string='';
    customerId: number=0;
    candidateId: number=0;

    applicationNo: number=0;
    candidateName: string='';
    referredOn: Date=new Date();
    selectedOn: Date=new Date();
    refStatus: number=0;
    deployStageId: number=0;
    deployStageName: string='';
    nextStageId: number=0;
    nextStageDate: Date=new Date();
    transactionDate: Date=new Date();
}