export interface IEmploymentDto
{
     applicationNo: number;
     candidateName: string;
     companyName: string;
     categoryRef: string;

     id: number;
     cVRefId: number;
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
     leavePerYearInDays: number;
     leaveAirfareEntitlementAfterMonths: number;
     offerAcceptedOn: Date;
     remarks: string;
}
