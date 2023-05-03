
export interface IUserAttachment {
     id: number;
     candidateId: number;
     appUserId: number;
     attachmentType: string;
     attachmentSizeInBytes: number;
     fileName: string;
     url: string;
     dateUploaded: Date;
     uploadedByEmployeeId: number;
}

export class UserAttachment implements IUserAttachment {
     id=0;
     candidateId=0;
     appUserId=0;
     attachmentType='';
     attachmentSizeInBytes = 0;
     fileName='';
     url = '';
     dateUploaded = new Date();
     uploadedByEmployeeId = 0;
}