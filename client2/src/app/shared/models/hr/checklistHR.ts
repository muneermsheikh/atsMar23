import { IChecklistHRItem } from "./checklistHRItem";

export interface IChecklistHR{

     id: number;
     candidateId: number;
     orderItemId: number;
     userId: number;
     checkedOn: Date;
     hrExecRemarks: string;
     charges: number;
     chargesAgreed: number;
     exceptionApproved: boolean;
     exceptionApprovedBy: string;
     exceptionApprovedOn: Date;

     checklistHRItems: IChecklistHRItem[];
}