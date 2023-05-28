import { DetachedRouteHandle } from "@angular/router";
import { IForwardedOfficialDto } from "./forwardedOfficialDto";

export interface IForwardedCategoryDto
{
    id: number;
    orderItemId: number;
    categoryRefAndName: string;
    charges: number;
    forwardedOfficials: IForwardedOfficialDto[];
}

export class forwardedCategoryDto implements IForwardedCategoryDto
{
    id: number=0;
    orderItemId: number=0;
    categoryRefAndName: string='';
    charges: number=0;
    forwardedOfficials: IForwardedOfficialDto[]=[];
}

