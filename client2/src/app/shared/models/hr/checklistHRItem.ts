export interface IChecklistHRItem
{
     id: number;
     checklistHRId: number;
     srNo: number;
     parameter: string;
     accepts: boolean;
     response: string;
     exceptions: string;
     mandatoryTrue: boolean;
}

export class ChecklistHRItem implements IChecklistHRItem
{
     id= 0;
     checklistHRId= 0;
     srNo= 0;
     parameter= '';
     accepts = false;
     response= '';
     exceptions= '';
     mandatoryTrue = false;
}