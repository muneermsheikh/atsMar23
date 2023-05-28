export interface IForwardedOfficialDto
{
    checked: boolean;
    forwardCategoryId: number;
    officialName: string;
    agentName: string;
    dateTimeForwarded: Date;
    dateOnlyForwarded: Date;
    emailIdForwardedTo: string;
    phoneNoForwardedTo: string;
    whatsAppNoForwardedTo: string;
    loggedInEmployeeId: number;
}

export class forwardedOfficialDto implements IForwardedOfficialDto
{
    checked=false;
    forwardCategoryId: number=0;
    officialName: string='';
    agentName: string='';
    dateTimeForwarded: Date = new Date('1900-01-01');
    dateOnlyForwarded: Date = new Date('1900-01-01');
    emailIdForwardedTo: string='';
    phoneNoForwardedTo: string='';
    whatsAppNoForwardedTo: string='';
    loggedInEmployeeId: number=0;

}