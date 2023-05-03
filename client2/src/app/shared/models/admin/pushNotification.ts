export interface IPushNotification
{
	addresseeType: string;
	addresseeId: number;
	notice: string;
}

export class PushNotification
{
	addresseeType: string;
	addresseeId: number;
	notice='';
}