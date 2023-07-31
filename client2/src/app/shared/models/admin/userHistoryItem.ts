export interface IUserHistoryItem 
{
     id: number;
     incomingOutgoing: string;
     userHistoryId: number;
     personType: string;
     personId: number;
     personName: string;
     phoneNo: string;
     subject: string;
     categoryRef: string;
     dateOfContact: Date;
     loggedInUserId: number;
     loggedInUserName: string;
     contactResultId: number;
     contactResultName: string;
     gistOfDiscussions: string;
     composeEmailMessage: boolean;
     composeSMS: boolean;
}
export class UserHistoryItem implements IUserHistoryItem 
{
     id = 0;
     incomingOutgoing = "in";
     userHistoryId = 0;
     personId = 0;
     personType="Candidate";
     personName = '';
     phoneNo = '';
     subject = '';
     categoryRef = '';
     dateOfContact = new Date();
     loggedInUserId = 0;
     loggedInUserName = '';
     contactResultId= 0 ;
     contactResultName = '';
     gistOfDiscussions = '';
     composeEmailMessage = false;
     composeSMS = false
}