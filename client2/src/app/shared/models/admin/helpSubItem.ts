export interface IHelpSubItem
{
	id: number;
	helpId: number;
	sequence: number;
	helpText: string;
	helpSubItems: IHelpSubItem[];
}