import { ITaskItem } from "./taskItem";

export interface IApplicationTask {
     id: number;
     taskTypeId: number;
     taskDate: Date;
     taskOwnerId: number;
     taskOwnerName: string;
     assignedToId: number;
     assignedToName: string;
     taskDescription: string;
     completeBy: Date;
     taskStatus: string;
     completedOn: Date;
     postTaskAction: number;
     historyItemId: number;

     orderId?: number;
     orderNo?: number;
     orderItemId?: number;
     applicationNo?: number;
     candidateId?: number;
     resumeId: string;
     taskItems: ITaskItem[];
}
export class ApplicationTask {
     id= 0;
     taskTypeId= 0;
     taskDate= new Date();
     taskOwnerId= 0;
     taskOwnerName= '';
     assignedToId= 0;
     assignedToName= '';
     taskDescription= '';
     completeBy=new Date();
     taskStatus= '';
     completedOn= new Date();
     postTaskAction= 0;
     historyItemId= 0;

     orderId= 0;
     orderNo= 0;
     orderItemId= 0;
     applicationNo= 0;
     candidateId= 0;
     resumeId= '';
     taskItems: ITaskItem[]=[];
}
