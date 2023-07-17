import { IVoucherAttachment } from "./voucherAttachment";
import { IVoucherEntry, VoucherEntry } from "./voucherEntry";

export interface IFinanceVoucher
{
	id: number;
	divn: string;
	coaId: number;
	accountName: string;
	voucherNo: number;
	voucherDated: Date;
	employeeId: number;
	reviewedById: number;
	reviewedByName: string;
	reviewedOn: Date;
	amount: number;
	narration: string;
	/*approved: boolean;
	approvedByEmployeeId: number;
	approvedOn: Date;
	*/
	voucherEntries: IVoucherEntry[];
	voucherAttachments: IVoucherAttachment[];
}

export class FinanceVoucher implements IFinanceVoucher
{
	id=0;
	divn = '';
	coaId = 0;
	accountName='';
	voucherNo = 0;
	voucherDated = new Date();
	employeeId = 0;
	reviewedById = 0;
	reviewedByName = '';
	reviewedOn=new Date();
	amount = 0;
	narration = '';
	/*approved= false;
	approvedByEmployeeId=0;
	approvedOn=new Date();
	*/
	voucherEntries: VoucherEntry[]=[];
	voucherAttachments: IVoucherAttachment[]=[];
}
