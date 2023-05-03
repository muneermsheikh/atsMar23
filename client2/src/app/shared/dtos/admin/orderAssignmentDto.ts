export interface IOrderAssignmentDto
{
     orderId: number;
     orderNo: number;
     orderDate: Date;
     cityOfWorking: string;
     projectManagerId: number;
     projectManagerPosition: string;
     orderItemId: number;
     hrExecId: number;
     categoryRef: string;
     categoryName: string;
     categoryId: number;
     customerId: number;
     customerName: string;
     quantity: number;
     completeBy: Date;
     postTaskAction: number;
}

export class orderAssignmentDto implements IOrderAssignmentDto
{
     orderId= 0;
     orderNo= 0;
     orderDate = new Date('1900-01-01');
     cityOfWorking= '';
     projectManagerId= 0;
     projectManagerPosition= '';
     orderItemId= 0;
     hrExecId= 0;
     categoryRef= '';
     categoryName= '';
     categoryId= 0;
     customerId= 0;
     customerName= '';
     quantity= 0;
     completeBy = new Date('1900-01-01');
     postTaskAction= 0;
}