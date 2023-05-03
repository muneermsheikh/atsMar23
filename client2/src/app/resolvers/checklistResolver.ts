import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ChecklistService } from "../candidate/checklist.service";
import { IChecklistHRDto } from "../shared/models/checklistHRDto";

@Injectable({
     providedIn: 'root'
 })
 export class ChecklistResolver implements Resolve<IChecklistHRDto> {
 
     constructor(private checklistService: ChecklistService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IChecklistHRDto> {
        return this.checklistService.getChecklist(+route.paramMap.get('candidateid'), +route.paramMap.get('orderitemid'));
     }
 
 }