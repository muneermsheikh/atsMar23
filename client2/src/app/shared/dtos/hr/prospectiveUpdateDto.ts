export interface IProspectiveUpdateDto
{
     prospectiveId: number;
     newStatus: string;
     remarks: string;
}

export class ProspectiveUpdateDto implements IProspectiveUpdateDto
{
     prospectiveId = 0;
     newStatus = '';
     remarks = '';
}