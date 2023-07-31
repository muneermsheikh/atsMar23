import { IUserHistoryItem } from "../../models/admin/userHistoryItem";

export interface IUserHistoryDto
{
     id: number;
     gender: string;
     userHistoryHeaderId?: number;
     checked?: boolean;
     source: string;
     categoryRef: string;
     categoryName: string;
     resumeId: string;
     name: string;
     personId?: number;
     personType: string;
     emailId: string;
     alternateEmailId: string;
     mobileNo:string;
     alternatePhoneNo: string;
     applicationNo?: number;
     createdOn: Date;
     concluded?: boolean;
     concludedByName: string;
     status: string;
     userName: string;
     errorMessage: string;
     userHistoryItems: IUserHistoryItem[];
}