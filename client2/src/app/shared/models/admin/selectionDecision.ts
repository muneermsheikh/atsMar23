export interface ISelectionDecision
{
     checked: boolean;
     id: number;
     cVRefId: number;
     orderItemId: number;
     categoryId: number;
     categoryRef: string;
     orderNo: number;
     customerName: string;
     applicationNo: number;
     candidateId: number;
     candidateName: string;
     decisionDate: Date;
     selectionStatusId: number;
     selectedOn: number;
     employment: IEmployment;
     remarks: string;
}

export interface IEmployment
{
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
     foodAlowance: number;
     transportProvidedFree: boolean;
     transportAllowance: number;
     otherAllowance: number;
     leavePerYearInDays: number;
     leaveAirfareEntitlementAfterMonths: number;
     offerAcceptedOn: Date;
     //new additions
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
     companyName: string;     //agent name
     remarks: string;
}
