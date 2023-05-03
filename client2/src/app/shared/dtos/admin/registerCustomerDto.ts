import { IAgencySpecialty } from "../../models/admin/agencySpecialty";
import { ICustomerIndustry } from "../../models/admin/customerIndustry";
import { ICustomerOfficialToCreateDto } from "./customerOfficialToCreateDto";

export interface IRegisterCustomerDto
{
     id: number;
     customerType: string;
     knownAs: string;
     add: string;
     add2: string;
     city: string;
     pin: string;
     district: string;
     state: string;
     country: string;
     email: string;
     website: string;
     phone: string;
     phone2: string;
     introduction: string;
     customerIndustries: ICustomerIndustry[];
     agencySpecialties: IAgencySpecialty[];
     customerOfficials: ICustomerOfficialToCreateDto[];
}

