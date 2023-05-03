export interface ISelPendingDto {
     id: number;
     checked: boolean;
     cvRefId: number;
     orderItemId: number;
     orderNo: number;
     customerName: string;
     categoryRefAndName: string;
     applicationNo: number;
     candidateName: string;
     referredOn: Date;
     refStatus: number;
     selectionStatusId: number;
     decisionDate: Date;
     remarks: string;
}