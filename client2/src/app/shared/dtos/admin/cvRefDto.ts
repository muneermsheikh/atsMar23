import { ICVRefDeployDto } from "./cvRefDeployDto";

export interface ICVRefDto
{
     cvRefId: number;
     customerName: string;
     categoryName: string;
     categoryRef: string;
     applicationNo: number;
     candidateName: string;
     referredOn: Date;
     selectedOn: Date;
     deployments: ICVRefDeployDto[];
}

