export interface IVoucherEntry{
	id: number;
	financeVoucherId: number;
	transDate: Date;
	coaId: number;
	dr: number;
	cr: number;
	accountName: string;
	narration: string;
}

export class VoucherEntry implements IVoucherEntry
{
	id=0;
	financeVoucherId=0;
	transDate=new Date();
	coaId=0;
	accountName = '';
	dr=0;
	cr=0;
	narration= '';
}