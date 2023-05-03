export interface IUser {
     loggedInEmployeeId: number;
     objectId: number;
     username: string;
     email: string;
     displayName: string;
     token: string;
     roles: string[];
}