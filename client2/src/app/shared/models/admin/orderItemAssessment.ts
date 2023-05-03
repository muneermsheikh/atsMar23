import { IOrderItemAssessmentQ } from "./orderItemAssessmentQ";

export interface IOrderItemAssessment
{
     id: number;
     orderAssessmentId: number;
     orderItemId: number;
     orderId: number;
     categoryId: number;
     categoryName: string;
     orderItemAssessmentQs: IOrderItemAssessmentQ[];
}