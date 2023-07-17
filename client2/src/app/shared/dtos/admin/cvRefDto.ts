import { ICVRefDeployDto } from "./cvRefDeployDto";
import { IDeployDto } from "./cvReferredDto";

export interface ICVRefDto
{
     id: number;
     cvRefId: number;
     customerName: string;
     categoryName: string;
     categoryRef: string;
     applicationNo: number;
     candidateName: string;
     referredOn: Date;
     selectedOn: Date;
     deployments: IDeployDto[];
}

export class CVRefDto implements ICVRefDto
{
     id = 0;
     cvRefId = 0;
     customerName = '';
     categoryName = '';
     categoryRef = '';
     applicationNo = 0;
     candidateName = '';
     referredOn = new Date('1900-01-01');
     selectedOn = new Date('1900-01-01');
     deployments: IDeployDto[]=[];
}

