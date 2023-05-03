export interface IMessage {
     id: number;
     messageGroup: string;
     messageTypeId: number;
     senderId: number;
     senderEmailAddress: string;
     senderUserName: string;
     recipientUserName: string;
     recipientEmailAddress: string;
     ccEmailAddress: string;
     bccEmailAddress: string;
     subject: string;
     content: string;
     messageComposedOn?: Date;
     dateReadOn?: Date;
     messageSentOn?: Date;
     senderDeleted: boolean;
     recipientDeleted: boolean;
     recipientId: number;

}

export class message {
     id: number=0;
     messageGroup: string='';
     messageTypeId: number=0;
     senderId: number=0;
     senderEmailAddress: string='';
     senderUserName: string='';
     recipientUserName: string='';
     recipientEmailAddress: string='';
     ccEmailAddress: string='';
     bccEmailAddress: string='';
     subject: string='';
     content: string='';
     messageComposedOn?: Date=new Date('1900-01-01');
     dateReadOn?: Date=new Date('1900-01-01');
     messageSentOn?: Date=new Date('1900-01-01');
     senderDeleted: boolean=false;
     recipientDeleted: boolean=false;
     recipientId: number=0;
}
