import { IAgencySpecialty } from "./agencySpecialty";
import { ICustomerIndustry } from "./customerIndustry";
import { ICustomerOfficial } from "./customerOfficial";
import { ICustomerOfficialDto } from "./customerOfficialDto";
import { IVendorSpecialty } from "./vendorSpecialty";


export interface ICustomer {
     id: number;
     customerType: string;
     customerName: string;
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
     logoUrl?: string;
     customerStatus: number;
     createdOn: Date;
     introduction: string;
     customerIndustries: ICustomerIndustry[];
     customerOfficials: ICustomerOfficial[];
     agencySpecialties: IAgencySpecialty[];
     vendorSpecialties: IVendorSpecialty[];
     lastActive: Date;
 }

 export class Customer implements ICustomer {
    id = 0;
    customerType= '';
    customerName= '';
    knownAs= '';
    add= '';
    add2= '';
    city= '';
    pin= '';
    district= '';
    state= '';
    country= '';
    email= '';
    website= '';
    phone= '';
    phone2= '';
    logoUrl?= '';
    customerStatus= 0;
    createdOn = new Date('1900-01-01');

    introduction= '';
    customerIndustries: ICustomerIndustry[]=[];
    customerOfficials: ICustomerOfficial[]=[];
    agencySpecialties: IAgencySpecialty[]=[];
    vendorSpecialties: IVendorSpecialty[]=[];
    
    lastActive = new Date('1900-01-01');
 }
