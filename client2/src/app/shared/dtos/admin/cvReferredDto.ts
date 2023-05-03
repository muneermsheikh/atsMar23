export interface ICVReferredDto
{
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
     deployments: IDeployDto[];
}

export interface IDeployDto
{
     id: number;
     cvRefId: number;
     transactionDate: Date;
     deploymentStatusName: string;
}