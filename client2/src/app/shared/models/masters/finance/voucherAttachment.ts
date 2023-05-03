
export interface IVoucherAttachment {
     id: number;
     voucherId: number;
     attachmentSizeInBytes: number;
     fileName: string;
     url: string;
     dateUploaded: Date;
     uploadedByEmployeeId: number;
}

export class VoucherAttachment implements IVoucherAttachment {
     id: number;
     voucherId: number;
     attachmentSizeInBytes: number;
     fileName: string;
     url: string;
     dateUploaded: Date;
     uploadedByEmployeeId: number;
}