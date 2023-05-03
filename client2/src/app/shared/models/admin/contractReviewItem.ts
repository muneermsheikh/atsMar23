import { IReviewItem } from "./reviewItem";

export interface IContractReviewItem
{
     id: number; 
     contractReviewId: number; 
     orderId: number; 
     orderNo: number;
     orderDate: Date;
     orderItemId: number; 
     customerName: string;
     categoryName: string; 
     quantity: number;
     ecnr: boolean; 
     requireAssess: boolean; 
     sourceFrom: string; 
     reviewItemStatus: number;
     reviewItems: IReviewItem[];
}