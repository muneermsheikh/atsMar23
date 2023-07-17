export interface IStatementofAccountDto
{
	accountId: number;
	accountName: string;
	fromDate: Date;
	uptoDate: Date;
	opBalance: number;
	clBalance: number;
	
	statementOfAccountItems: IStatementofAccountItemDto[];

}

export interface IStatementofAccountItemDto
{
	voucherNo: number;
	transDate: Date;
	coaId: number;
	accountName: string;
	dr: number;
	cr: number;
	narration: string;

}