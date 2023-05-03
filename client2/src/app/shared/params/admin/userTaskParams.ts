export class userTaskParams {
     taskId?: number;
     taskTypeId: number=0;
     taskDate = new Date('1900-01-01');
     candidateId: number=0;
     applicationNo: number=0;
     resumeId: string='';
     personType: string='';
     assignedToId?: number;
     assignedToNameLike?: string;
     taskStatus? ='';
     orderId? = 0;
     
     sort = "taskDate";
     pageNumber = 1;
     pageSize = 15;
     search: string='';
}