import { IDLForwardCategory } from "./dlForwardCategory";

export interface IDLForwardToAgent
{
	id: number;
	orderId: number;
	orderNo: number;
	orderDate: Date;
	customerId: number;
	customerName: string;
	customerCity: string;
	projectManagerId: number;
	dlForwardCategories: IDLForwardCategory[];
}

export class dLForwardToAgent 
{
	id= 0;
	orderId= 0;
	orderNo= 0;
	orderDate: Date = new Date('1900-01-01');
	customerId= 0;
	customerName= '';
	customerCity= '';
	projectManagerId= 0;
	dlForwardCategories: IDLForwardCategory[]=[];
}

