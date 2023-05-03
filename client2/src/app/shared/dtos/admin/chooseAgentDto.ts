export interface IChooseAgentDto
{
     
     customerId: number;
     customerName: string;
     city: string;
     officialId: number;
     officialName: string;
     designation: string;
     title: string;
     officialEmailId: string;
     phoneNo: string;
     mobileNo: string;
     value: number;
     checked: boolean;
     checkedPhone: boolean;
     checkedMobile: boolean; 
     isBlocked: boolean;
}

export class chooseAgentDto implements IChooseAgentDto
{
     customerId= 0;
     customerName= '';
     city= '';
     officialId= 0;
     officialName= '';
     designation= '';
     title= '';
     officialEmailId= '';
     phoneNo= '';
     mobileNo= '';
     value= 0;
     checked = false;
     checkedPhone = false;
     checkedMobile = false; 
     isBlocked = false;
}