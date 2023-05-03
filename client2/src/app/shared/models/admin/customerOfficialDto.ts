export interface ICustomerOfficialDto
{
     officialId: number;
     customerId: number;
     customerName: string;
     city: string;
     title: string;
     officialName: string;
     designation: string;
     officialEmailId: string;
     mobileNo: string;
     checked: boolean;
     checkedPhone: boolean;
     checkedMobile: boolean;
     customerIsBlacklisted: boolean;
}

export class CustomerOfficialDto implements ICustomerOfficialDto
{
     officialId=0;
     customerId= 0;
     customerName= '';
     city= '';
     title= '';
     officialName= '';
     designation= '';
     officialEmailId='';
     mobileNo='';
     checked=false;
     checkedPhone=false;
     checkedMobile=false;
     customerIsBlacklisted: boolean = false;
}