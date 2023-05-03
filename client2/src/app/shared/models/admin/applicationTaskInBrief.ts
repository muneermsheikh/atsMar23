export interface IApplicationTaskInBrief {
     id: number;
     taskTypeId: number;

     taskDate: Date;
     taskOwnerId: number;
     assignedToId: number;
     
     orderId: number;
     orderNo: number;
     orderItemId: number;
     applicationNo: number;
     candidateId: number;
     taskDescription: string;
     completeBy: Date;
     taskStatus: string;
     completedOn: Date;
     postTaskActionName: string;
     historyItemId: number;

     //additional fields
     taskTypeName: string;
     taskOwnerName: string;
     assignedToName: string;
     

     
}

export class ApplicationTaskInBrief implements IApplicationTaskInBrief {
     id=0;
     candidateId= 0;
     taskTypeId= 0;
     taskTypeName= '';
     taskDate = new Date('1900-01-01');
     taskOwnerId= 0;
     taskOwnerName= '';
     assignedToId= 0;
     assignedToName= '';
     taskDescription= '';
     completeBy = new Date('1900-01-01');
     taskStatus= '';
     completedOn =  new Date('1900-01-01');
     postTaskActionName= '';
     historyItemId= 0;

     orderId= 0;
     orderNo= 0;
     orderItemId= 0;
     applicationNo= 0;
}