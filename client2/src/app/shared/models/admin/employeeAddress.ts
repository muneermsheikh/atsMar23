export interface IEmployeeAddress {
     addressType: string;
     add: string;
     streetAdd: string;
     city: string;
     pin: string;
     state: string;
     district: string;
     country: string;
     isMain: boolean;
     employeeId: number;
     id: number;
}

export class EmployeeAddress implements IEmployeeAddress {
     addressType = '';
     add = '';
     streetAdd = '';
     city = '';
     pin =  '';
     state = '';
     district =  '';
     country = '';
     isMain = false;
     employeeId= 0;
     id = 0;
}