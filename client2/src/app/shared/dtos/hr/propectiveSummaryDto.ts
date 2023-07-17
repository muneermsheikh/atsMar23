export interface IProspectiveSummaryDto
{
	categoryRef: string;
	status: string;
	source: string;
	date: Date;
	pending: number;
	concluded: number;
	wrongNo: number;
	lowSalary: number;
	askedToContactLater: number;
	phoneNoWrong: number;
	phoneNotReachable: number;
	notResponding: number;
	notInterested: number;
	scNotAcceptable: number;
	ppIssues: number;
	others: number;
	total: number;
}