export interface IOrderBriefDto
{
     id: number;
     orderNo: number;
     orderId: number;
     orderDate: Date;
     completeBy: Date;
     customerId: number;
     customerName: string;
     cityOfWorking: string;
     status: string;
     contractReviewStatusId: number;
     contractReviewId: number;
     reviewStatus: string;
     reviewedBy: number;
     reviewedOn: Date;
     forwardedToHRDeptOn: Date;
     
}

export interface IOrderItemDto{
     checked: boolean;
     orderId: number;
     orderItemId: number;
     categoryRef: string;
     categoryName: string;
     quantity: number;
     status: string;
}

export class OrderBriefDto 
{
     id = 0;
     orderNo = 0;
     orderDate = new Date('1900-01-01');
     customerName = '';
     contractReviewStatusId = 0;
     items: IOrderItemDto[]=[];

}