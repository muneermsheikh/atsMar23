import { ICustomerReviewItem } from "./customerReviewItem";

export interface ICustomerReview
{
     id: number;
     customerId: number;
     customerName: string;
     city: string;
     currentStatus: string;
     remarks: string;
     customerReviewItems: ICustomerReviewItem[];
}