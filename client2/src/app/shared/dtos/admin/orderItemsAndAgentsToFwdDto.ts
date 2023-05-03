
import { ICustomerOfficialDto } from "../../models/admin/customerOfficialDto";
import { IOrderItemToFwdDto } from "./orderItemToFwdDto";

export interface IOrderItemsAndAgentsToFwdDto
{
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     projectManagerId: number;
     items: IOrderItemToFwdDto[];
     agents: ICustomerOfficialDto[];
     dateForwarded: Date;
}

export class OrderItemsAndAgentsToFwdDto implements IOrderItemsAndAgentsToFwdDto
{
     orderId= 0;
     orderNo= 0;
     orderDate = new Date('1900-01-01');
     customerId= 0;
     customerName= '';
     projectManagerId= 0;
     items: IOrderItemToFwdDto[]=[];
     agents: ICustomerOfficialDto[]=[];
     dateForwarded = new Date('1900-01-01');
}