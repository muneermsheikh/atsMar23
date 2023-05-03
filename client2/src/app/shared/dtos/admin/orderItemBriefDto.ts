
export interface IOrderItemBriefDto
{
     checked: boolean;
     orderNo: number;
     orderDate: Date;
     customerName: string;
     orderItemId: number;
     requireInternalReview: boolean;
     categoryRef: string;
     categoryName: string;
     categoryRefAndName: string;
     quantity: number;
     status: string;
     assessmentQDesigned: boolean;
}

export class OrderItemBriefDto implements IOrderItemBriefDto
{
     checked: boolean = false;
     orderNo = 0;
     orderDate= new Date();
     customerName='';
     orderItemId=0;
     requireInternalReview = false;
     categoryId = 0;
     categoryRef='';
     categoryName = '';
     categoryRefAndName='';
     quantity=0;
     status='';
     assessmentQDesigned = false;
}