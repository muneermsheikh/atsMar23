export interface IOpenOrderItemCategoriesDto
{
     checked: boolean;
     orderNo: number;
     orderDate: Date;
     customerName: string;
     orderItemId: number;
     categoryRefAndName: string;
     quantity: number;
}

export class OpenOrderItemCategoriesDto implements IOpenOrderItemCategoriesDto
{
     checked: boolean = false;
     orderNo = 0;
     orderDate = new Date('1900-01-01');
     customerName = '';
     orderItemId = 0;
     categoryRefAndName = '';
     quantity = 0;
}