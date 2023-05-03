export interface IUserPassport {
     id: number;
     candidateId: number;
     passportNo: string;
     nationality: string;
     issuedOn?: Date;
     validity: Date;
     isValid: boolean;
     ecnr: boolean;
}