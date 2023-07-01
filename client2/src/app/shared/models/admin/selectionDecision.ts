import { IEmployment } from "./employment";

export interface ISelectionDecision
{
     checked: boolean;
     id: number;
     cVRefId: number;
     orderItemId: number;
     categoryId: number;
     categoryRef: string;
     orderNo: number;
     customerName: string;
     applicationNo: number;
     candidateId: number;
     candidateName: string;
     decisionDate: Date;
     selectionStatusId: number;
     selectedOn: number;
     employment: IEmployment;
     remarks: string;
}

