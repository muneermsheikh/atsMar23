export class EmailMessageSpecParams {
     id?: number;
     username: string='';
     container: string='';
     senderEmail: string='';
     recipientEmail: string='';
     messageTypeId: number=0;
     sort = "messageSent";
     pageIndex = 1;
     pageSize = 1;
     search: string='';

}