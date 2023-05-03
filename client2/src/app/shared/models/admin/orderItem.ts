import { IContractReviewItem } from "./contractReviewItem";
import { IJobDescription } from "./jobDescription";
import { IRemuneration } from "./remuneration";

export interface IOrderItem{
     selected: boolean;
     id: number; 
     orderId: number;
     orderNo: number; 
     srNo: number; 
     categoryId: number;
     categoryName: string; 
     ecnr: boolean; 
     isProcessingOnly: boolean;
     industryId: number; 
     industryName: string;
     sourceFrom: string; 
     quantity: number; 
     minCVs: number; 
     maxCVs: number;
     requireInternalReview: boolean; 
     requireAssess: boolean; 
     completeBefore: Date; 
     hrExecId: number; 
     hrExecName: string;
     hrSupId: number; 
     hrSupName: string; 
     noReviewBySupervisor: boolean; 
     hrmId: number; 
     hrmName: string, 
     assignedId: number; 
     assignedToName: string; 
     charges: number; 
     feeFromClientINR: number; 
     status: string;
     checked: boolean;
     reviewItemStatusId: number; 
     jobDescription: IJobDescription; 
     remuneration: IRemuneration;
     contractReviewItem: IContractReviewItem;
}