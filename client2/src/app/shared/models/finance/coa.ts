export interface ICOA{
	id: number;
	divn: string;
	accountType: string;
	accountName: string;
	accountClass: string;
	opBalance: number;
}

export class coa implements ICOA{
	id=0;
	divn='';
	accountType='';
	accountName='';
	accountClass='';
	opBalance=0;
}