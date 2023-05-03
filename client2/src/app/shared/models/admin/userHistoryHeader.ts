
import { IUserHistory } from "./userHistory";

export interface IUserHistoryHeader
{
	checked: boolean;
	id: number;
	categoryRefCode: string;
	categoryRefName: string;
	customerName: string;
	completeBy: Date;
	assignedToId: number;
	assignedToName: string;
	assignedById: number;
	assignedByName: string;
	assignedOn: Date;
	status: string;
	concluded: boolean;
	userHistories: IUserHistory[];
}