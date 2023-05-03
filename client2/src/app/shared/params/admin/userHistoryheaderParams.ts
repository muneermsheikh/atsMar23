export class userHistoryHeaderParams {
     id?=0;
     categoryRefCode?: string;
     categoryRefName?: string;
     customerName?: string;
     assignedToId?: number;
     assignedById?: number;
     status?: string;
     
     sort = "name";
     pageNumber = 1;
     pageSize = 10;
     search: string='';
}