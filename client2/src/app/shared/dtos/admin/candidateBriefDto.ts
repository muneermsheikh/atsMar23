import { IUserProfession } from "../../params/admin/userProfession";

export interface ICandidateBriefDto {
     checked: boolean;
     id: number;
     historyId: number;
     mobileNo: string;
     gender: string;
     applicationNo: number;
     aadharNo: string;
     fullName: string;
     city: string;
     referredById: number;
     referredByName: string;
     candidateStatusName: string;
     userProfessions: IUserProfession[];
}

export class CandidateBriefDto implements ICandidateBriefDto {
     checked=false;
     id = 0;
     historyId = 0;
     mobileNo = '';
     gender='male';
     applicationNo = 0;
     aadharNo = '';
     fullName = '';
     city= '';
     referredById = 0;
     referredByName = '';
     candidateStatusName = '';
     userProfessions: IUserProfession[]=[];
}