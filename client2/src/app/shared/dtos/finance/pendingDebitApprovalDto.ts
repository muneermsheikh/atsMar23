export interface IPendingDebitApprovalDto
{
	voucherEntryId: number;
	voucherNo: number;
	voucherDated: Date;
	drAccountId: number;
	drAccountName: string;
	drAmount: number;
	drEntryApproved: boolean;
	drEntryApprovedByEmployeeId: number;
	drEntryApprovedOn: Date;

}

export class PendingDebitApprovalDto implements IPendingDebitApprovalDto
{
	voucherEntryId = 0;
	voucherNo=0;
	voucherDated=new Date();
	drAccountId = 0;
	drAccountName = '';
	drAmount = 0;
	drEntryApproved = false;
	drEntryApprovedByEmployeeId = 0;
	drEntryApprovedOn = new Date();
}