export class qBankParams {
     categoryId?: number;
     assessmentParameter?: string;
     isStandardQ?: boolean;
     
     sort = "name";
     pageNumber = 1;
     pageSize = 10;
     search: string='';
}