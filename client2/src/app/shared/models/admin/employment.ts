export interface IEmployment
{
	id: number;
	cVRefId: number;
	selectionDecisionId: number;

	selectedOn: Date;
	charges: number;
	salaryCurrency: string;
	salary: number;
	contractPeriodInMonths: number;
	weeklyHours: number;
	housingProvidedFree: boolean;
	housingAllowance: number;
	foodProvidedFree: boolean;
	foodAllowance: number;
	transportProvidedFree: boolean;
	transportAllowance: number;
	otherAllowance: number;
	leavePerYearInDays: number;
	leaveAirfareEntitlementAfterMonths: number;
	offerAcceptedOn: Date;

	categoryId: number;
	categoryName: string;
	orderItemId: number;
	orderId: number;
	orderNo: number;
	customerId: number;
	customerName: string;
	candidateId: number;
	applicationNo: number;
	candidateName: string;
	agentName: string;
	approved: boolean;
	approvedByEmpId:number;
	approvedOn: Date;

}