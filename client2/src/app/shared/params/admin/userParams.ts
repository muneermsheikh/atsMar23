export class UserParams {
     userType: string='';
     username: string='';
     email: string='';
     displayName: string='';
     token: string='';
     roles: string[]=[];

     sort = "name";
     pageNumber = 1;
     pageSize = 10;
     search: string='';
}