import { IForwardedCategoryDto } from "./forwardedCategoryDto";

export interface IForwardedDateDto
{
    orderNo: number;
    orderDate: Date;
    customerName: string;
    forwardedCategories: IForwardedCategoryDto[];
}

export class ForwardedDateDto implements IForwardedDateDto
{
    orderNo = 0;
    orderDate = new Date('1900-01-01');
    customerName='';
    forwardedCategories: IForwardedCategoryDto[]=[];
}
