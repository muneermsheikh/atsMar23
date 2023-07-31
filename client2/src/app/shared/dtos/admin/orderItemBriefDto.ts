
export interface IOrderItemBriefDto
{
     srNo: number;
     checked: boolean;
     orderNo: number;
     orderDate: Date;
     customerName: string;
     orderItemId: number;
     requireInternalReview: boolean;
     categoryId: number;
     categoryRef: string;
     categoryName: string;
     categoryRefAndName: string;
     quantity: number;
     status: string;
     assessmentQDesigned: boolean;
}

export class OrderItemBriefDto implements IOrderItemBriefDto
{
     srNo = 0;
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