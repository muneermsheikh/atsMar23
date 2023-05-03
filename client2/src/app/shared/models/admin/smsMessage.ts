import { DateSelectionModelChange } from "@angular/material/datepicker";

export interface smsMessage {
	sequenceNo: number;
	userId: number;
	smsDateTime: Date;
	phoneNo: string;
	smsText: string;
	deliveryResult: string;
}