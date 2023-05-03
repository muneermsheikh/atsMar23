export interface IUserRoleObj
{
     email: string;
     name: string;
     value: string;
     checked: boolean;
}

export class userRoleObj implements IUserRoleObj
{
     email: string='';
     name: string='';
     value: string='';
     checked: boolean=false;
}