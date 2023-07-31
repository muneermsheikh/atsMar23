export interface IOrderItemAssessmentQ
{
     id: number;
     orderAssessmentItemId: number;
     orderItemId: number;
     orderId: number;
     questionNo: number;
     subject: string;
     question: string;
     maxPoints: number;
     isMandatory: boolean;
}