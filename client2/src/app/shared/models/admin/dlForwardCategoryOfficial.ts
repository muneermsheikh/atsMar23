export interface IDLForwardCategoryOfficial
{
	checked: boolean;
	id: number;
	dLForwardItemId: number;
	orderItemId: number;
	customerOfficialId: number;
	agentName: string;
	dateTimeForwarded: Date;
	dateOnlyForwarded: Date;
	emailIdForwardedTo: string;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo: String;
	loggedInEmployeeId: number;
}

export class dLForwardCategoryOfficial implements IDLForwardCategoryOfficial
{
	checked = false;
	id= 0;
	dLForwardItemId= 0;
	orderItemId= 0;
	customerOfficialId= 0;
	agentName= '';
	dateTimeForwarded= new Date('1900-01-01');
	dateOnlyForwarded= new Date('1900-01-01');
	emailIdForwardedTo= '';
	phoneNoForwardedTo= '';
	whatsAppNoForwardedTo= '';
	loggedInEmployeeId= 0;
}