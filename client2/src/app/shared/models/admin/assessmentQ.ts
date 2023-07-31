export interface IAssessmentQ {
     id:  number;
     assessmentId: number;
     orderItemId: number;
     orderId: number;
     questionNo: number;
     subject: string;
     question: string;
     maxPoints: number;
     isMandatory: boolean;
}