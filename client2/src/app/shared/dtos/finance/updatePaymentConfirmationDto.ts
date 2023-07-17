export interface IUpdatePaymentConfirmationDto
{
	voucherEntryId: number;
	drEntryApproved: boolean;
	drEntryApprovedOn: Date;
	drEntryApprovedByEmployeeId: number;
}

export class UpdatePaymentConfirmationDto
{
	voucherEntryId= 0;
	drEntryApproved=false;
	drEntryApprovedOn= new Date();
	drEntryApprovedByEmployeeId=0;
}