export interface IMenuHelp
{
	id: number;
	menu: string;
	menuUrl: string;
	menuItems: IMenuItemHelp[];
}

export interface IMenuItemHelp
{
	id: number;
	subMenu: string;
	menuUrl: string;
	subMenuItems: IMenuSubItemHelp[];
}

export interface IMenuSubItemHelp
{
	id: number;
	subMenuItem: string;
	menuUrl: string;
}