import { dLForwardCategoryOfficial, IDLForwardCategoryOfficial } from "./dlForwardCategoryOfficial";

export interface IDLForwardCategory
{
	checked: boolean;
	id: number;
	orderId: number;
	orderItemId: number;
	dlForwardToAgentId: number;
	categoryId: number;
	categoryName: string;
	charges: number;
	dlForwardCategoryOfficials: IDLForwardCategoryOfficial[]
}

export class dLForwardCategory implements IDLForwardCategory
{
	checked= false;
	id= 0;
	orderId= 0;
	orderItemId= 0;
	dlForwardToAgentId= 0;
	categoryId= 0;
	categoryName= '';
	charges= 0;
	dlForwardCategoryOfficials: dLForwardCategoryOfficial[]=[];
}

//teachers: forwards, batches: category, students: officials
//teachersForm: forwardsForm