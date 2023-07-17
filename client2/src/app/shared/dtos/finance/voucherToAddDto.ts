import { IVoucherEntry } from "../../models/finance/voucherEntry";

export interface IVoucherToAddDto
{
    id: number;
    divn: string;
    voucherNo: number;
    voucherDated: Date;
    coaId: number;
    amount: number;
    narration: string;
    loggedInName: string;
    voucherEntries: IVoucherEntry[];
}

export class voucherToAddDto
{
    id=0;
    divn='';
    voucherNo=0;
    voucherDated=new Date();
    coaId = 0;
    amount=0;
    narration='';
    loggedInName='';
    voucherEntries: IVoucherEntry[]=[];
}