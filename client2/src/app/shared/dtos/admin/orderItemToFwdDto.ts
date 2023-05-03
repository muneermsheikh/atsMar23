import { IDLForwardDate } from "../../models/admin/dlForwardDate";

export interface IOrderItemToFwdDto
{
     id: number;
     orderItemId: number;
     orderId: number;
     orderDate: Date;

     categoryId: number;
     categoryRef: string;
     categoryName: string;
     quantity: number;
     charges: number;
     dlForwardDates: IDLForwardDate[];
}

export class OrderItemToFwdDto implements IOrderItemToFwdDto
{
     id= 0;
     orderItemId= 0;
     orderId= 0;
     orderDate= new Date('1900-01-01');
     
     categoryId= 0;
     categoryRef= '';
     categoryName= '';
     quantity= 0;
     charges= 0;
     dlForwardDates: IDLForwardDate[]=[];
}