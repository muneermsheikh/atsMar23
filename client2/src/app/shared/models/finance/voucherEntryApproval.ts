export interface IVoucherEntryApproval
{
	id: number;
	voucherEntryId: number;
	approved: boolean;
	approvedByEmployeeId: number;
	approvedOn: Date;
}

export class VoucherEntryApproval implements IVoucherEntryApproval
{
	id = 0;
	voucherEntryId = 0;
	approved = false;
	approvedByEmployeeId = 0;
	approvedOn = new Date();
}