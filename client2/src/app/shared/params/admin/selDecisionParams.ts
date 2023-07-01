export class SelDecisionParams
{
     id?: number=0;
     orderItemId: number=0;
     candidateId?: number=0;
     orderId?: number=0;
     orderNo?: number=0;
     applicationNo?: number=0;
     customerName: string='';
     categoryId: number=0;
     categoryName: string='';
     candidateName: string='';
     referredOn?: Date;
     cVRefId?: number=0;
     cvRefStatus?: number=0;
     cVRefIds: number[]=[];
     ids: number[]=[];
     includeSelection: boolean=false;
     includeEmployment: boolean=false;
     includeDeployment: boolean=false;
     
     pageIndex = 1;
     pageSize=3;
     sort = "";
     search: string='';
}