export interface ITaskItem {
     id: number;
     applicationTaskId: number;
     transactionDate: Date;
     taskTypeId: number;
     taskStatus: string;
     taskItemDescription: string;
     employeeId: number;
     orderId: number;
     orderNo: number;
     candidateId: number;
     userId: number;
     userName: string;
     quantity: number;
     nextFollowupOn: Date;
     nextFollowupById?: number;
}