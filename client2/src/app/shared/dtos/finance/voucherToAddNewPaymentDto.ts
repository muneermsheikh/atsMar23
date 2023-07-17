export interface IVoucherToAddNewPaymentDto
{
	debitCOAId: number;
	debitAccountName: string;
	creditCOAId: number;
	creditAccountName: string;
	amount: number;
	paymentDate: Date;
	narration: string;
	drEntryRequiresApproval: boolean;
}

export class VoucherToAddNewPaymentDto implements IVoucherToAddNewPaymentDto
{
	debitCOAId = 0;
	debitAccountName = '';
	creditCOAId = 0;
	creditAccountName = '';
	amount= 0;
	paymentDate= new Date('1900-01-01');
	narration = '';
	drEntryRequiresApproval = false;
}