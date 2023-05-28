import { IOrderItem } from "./orderItem";

export interface IOrder 
{
     id: number; 
     orderNo: number; 
     orderDate: Date; 
     customerId: number; 
     customerName: string;
     buyerEmail: string; 
     orderRef: string; 
     orderRefDate: Date;
     salesmanId: number; 
     salesmanName: string; 
     projectManagerId: number;
     medicalProcessInchargeEmpId: number; 
     visaProcessInchargeEmpId: number; 
     emigProcessInchargeId: number;
     travelProcessInchargeId: number;
     completeBy: Date; 
     country: string; 
     cityOfWorking: string; 
     contractReviewStatusId: number;
     estimatedRevenue: number;
     status: string; 
     forwardedToHRDeptOn: Date; 
     orderItems: IOrderItem[];
     
}

export class Order implements IOrder
{
     id=0; 
     orderNo= 0; 
     orderDate = new Date('1900-01-01'); 
     customerId= 0; 
     customerName= '';
     buyerEmail= ''; 
     orderRef= ''; 
     orderRefDate= new Date;
     salesmanId= 0; 
     salesmanName= ''; 
     projectManagerId= 0;
     medicalProcessInchargeEmpId= 0; 
     visaProcessInchargeEmpId= 0; 
     emigProcessInchargeId= 0;
     travelProcessInchargeId= 0;
     completeBy= new Date; 
     country= ''; 
     cityOfWorking= ''; 
     contractReviewStatusId= 0;
     estimatedRevenue= 0;
     status= ''; 
     forwardedToHRDeptOn= new Date('1900-01-01'); 
     orderItems: IOrderItem[]=[];
     
}