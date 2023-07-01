import { IChecklistHRItem } from "../../models/hr/checklistHRItem";


export interface IChecklistHRDto{

     id: number;
     candidateId: number;
     applicationNo: number;
     candidateName: string;
     categoryRef: string;
     orderRef: string;
     orderItemId: number;
     userLoggedId: number;
     userLoggedName: string;
     checkedOn: Date;
     hrExecComments: string;
     charges: number;
     chargesAgreed: number;
     exceptionApproved: boolean;
     exceptionApprovedBy: string;
     exceptionApprovedOn: Date;
     checklistedOk: boolean;
     assessmentIsNull: boolean;
     requireInternalReview: boolean;
     checklistHRItems: IChecklistHRItem[];
}

export class ChecklistHRDto implements IChecklistHRDto{

     id: number=0;
     candidateId: number=0;
     applicationNo: number=0;
     orderItemId: number=0;
     categoryRef: string='';
     candidateName: string='';
     orderRef: string='';
     userLoggedId: number=0;
     userLoggedName: string='';
     checkedOn: Date = new Date('1900-10-10');
     hrExecComments: string='';
     charges: number=0;
     chargesAgreed: number=0;
     exceptionApproved: boolean=false;
     exceptionApprovedBy: string='';
     exceptionApprovedOn: Date=new Date('1900-01-01');
     checklistedOk: boolean=false;
     assessmentIsNull = false;
     requireInternalReview=false;
     checklistHRItems: IChecklistHRItem[]=[];
}