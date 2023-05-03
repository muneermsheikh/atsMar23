export interface IOrderBrief{
     id: number; 
     contractReviewId: number;
     orderId: number;
     orderNo: number; 
     orderDate: Date; 
     customerId: number; 
     customerName: string;
     cityOfWorking: string; 
     country: string; 
     completeBy: Date; 
     contractReviewStatusId: number;
     reviewStatus: string;
     reviewedBy: string;
     reviewedOn: Date;
     status: number; 
     orderStatus: string;
     forwardedToHRDeptOn: Date; 
}