export interface IRemuneration
{
     id: number; orderItemId: number; orderId: number; orderNo: number; categoryId: number; workHours: number;
     salaryCurrency: string; salaryMin: number; salaryMax: number; contractPeriodInMonths: number;
     housingProvidedFree: boolean; housingAllowance: number; housingNotProvided: boolean;
     foodProvidedFree: boolean; foodAllowance: number; foodNotProvided: boolean;
     transportProvidedFree: boolean; transportAllowance: number; transportNotProvided: boolean;
     otherAllowance: number; leavePerYearInDays: number; leaveAirfareEntitlementAfterMonths: number;
}