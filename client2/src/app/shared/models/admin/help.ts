import { IHelpItem } from "./helpItem";

export interface IHelp
{
	id: number;
	topic: string;
	helpItems: IHelpItem[];
}

