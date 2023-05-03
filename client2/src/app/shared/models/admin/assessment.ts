import { IAssessmentQ } from "./assessmentQ";

export interface IAssessment {
     id: number;
     orderAssessmentId: number;
     orderItemId: number;
     orderId: number;
     orderNo: number;
     categoryId: number;
     categoryName: string;
     orderItemAssessmentQs: IAssessmentQ[];
}
