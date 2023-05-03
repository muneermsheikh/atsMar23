export interface ICustomerReviewItem{
     id: number;
     customerReviewId: number;
     reviewTransactionDate: Date;
     userId: number;
     customerReviewDataId: number;
     remarks: string;
     approvedBySup: boolean;
     approvedById: number;
}