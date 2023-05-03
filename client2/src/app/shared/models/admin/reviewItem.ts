export interface IReviewItem{
     id: number;
     orderItemId: number;
     contractReviewItemId: number;
     srNo: number;
     reviewParameter: string;
     response: boolean;
     isResponseBoolean: boolean;
     responseText: string;
     isMandatoryTrue: boolean;
     remarks: string;
}