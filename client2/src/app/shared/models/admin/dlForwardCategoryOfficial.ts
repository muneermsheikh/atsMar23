export interface IDLForwardCategoryOfficial
{
	checked: boolean;
	id: number;
	dLForwardCategoryId: number;
	orderItemId: number;
	customerOfficialId: number;
	officialName: string;
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
	dLForwardCategoryId= 0;
	orderItemId= 0;
	customerOfficialId= 0;
	officialName = '';
	agentName= '';
	dateTimeForwarded= new Date('1900-01-01');
	dateOnlyForwarded= new Date('1900-01-01');
	emailIdForwardedTo= '';
	phoneNoForwardedTo= '';
	whatsAppNoForwardedTo= '';
	loggedInEmployeeId= 0;
}