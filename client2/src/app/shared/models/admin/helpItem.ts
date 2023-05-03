import { IHelpSubItem } from "./helpSubItem";

export interface IHelpItem
{
	id: number;
	helpId: number;
	sequence: number;
	helpText: string;
	helpSubItems: IHelpSubItem[];
}