export interface IEmployment
{
	id: number;

	applicationNo: number;
	candidateName: string;
	categoryName: string;
	customerName: string;
	orderNo: number;
	weeklyWorkHours: number;
	leavePerYearInDays: number;
	leaveAirfareEntitlementAfterMonths: number;
	offerAcceptedOn: Date;
	remarks: string;
	
	cvRefId: number;
	selectionDecisionId: number;
	selectedOn: Date;
	charges: number;
	salaryCurrency: string;
	salary: number;
	contractPeriodInMonths: number;
	housingProvidedFree: boolean;
	housingAllowance: number;
	foodProvidedFree: boolean;
	foodAlowance: number;
	transportProvidedFree: boolean;
	transportAllowance: number;
	otherAllowance: number;
}