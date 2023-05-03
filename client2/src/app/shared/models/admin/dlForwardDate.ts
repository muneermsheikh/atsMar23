export interface IDLForwardDate
{
	id: number;
	dlForwardItemId: number;
	orderItemId: number;
	customerOfficialId: number;
	dateTimeForwarded: Date;
	dateOnlyForwarded: Date;
	emailIdForwardedTo: string;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo: string;
	loggedInEmployeeId: number;
}

export class DLForwardDate
{
	id: number;
	dlForwardItemId: number;
	orderItemId: number;
	customerOfficialId: number;
	dateTimeForwarded: Date;
	dateOnlyForwarded: Date;
	emailIdForwardedTo: string;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo: string;
	loggedInEmployeeId: number;
}