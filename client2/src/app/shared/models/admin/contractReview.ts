import { IContractReviewItem } from "./contractReviewItem";

export interface IContractReview{
     id: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     reviewedBy?: number;
     reviewedOn: Date;
     rvwStatusId: number;          //enumReviewStatus
     releasedForProduction: boolean;
     contractReviewItems: IContractReviewItem[];
}