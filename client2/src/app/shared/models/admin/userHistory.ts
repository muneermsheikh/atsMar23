import { IUserHistoryItem } from "./userHistoryItem";

export interface IUserHistory 
{
     
     id: number;
     gender: string;
     age: string;
     userHistoryHeaderId?: number;
     checked?: boolean;
     source: string;
     categoryRef: string;
     resumeId: string;
     nationality: string;
     address: string;
     currentLocation: string;
     city: string;
     name: string;
     personType: string;
     personId?: number;
     emailId: string;
     alternateEmailId: string;
     mobileNo: string;
     alternatePhoneNo: string;
     education: string;
     ctc: string;
     workExperience: string;
     applicationNo?: number;
     createdOn: Date;
     concluded?: boolean;
     status: string;
     statusDate?: Date;
     userName: string;
     concludedOn?: Date;
     concludedById?: number;
     userHistoryItems: IUserHistoryItem[];
}