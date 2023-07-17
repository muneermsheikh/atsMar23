export interface IVoucherDto
{
	id: number;
	divn: string;
	accountName: string;
	voucherNo: number;
	voucherDated: Date;
	amount: number;
	narration: string;
	approved: boolean;
    reviewedByName: string;
	reviewedOn: Date;
	loggedInName: string;
}