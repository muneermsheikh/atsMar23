export interface IUserHistoryHeaderDto
{
	id: number;
	categoryRefCode: string;
	categoryRefName: string;
	customerName: string;
	completeBy: Date;
	assignedToName: string;
	assignedByName: string;
	assignedOn: Date;
	status: string;
	concluded: boolean;
	totalCount: number;
	totalNotContactible: number;
	totalContacted: number;
	totalPositive: number;
	totalNegative: number;

}