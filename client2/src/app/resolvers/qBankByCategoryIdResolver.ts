import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { HrService } from "../hr/hr.service";
import { IAssessmentQBank } from "../shared/models/assessmentQBank";

@Injectable({
     providedIn: 'root'
 })
 export class QBankByCategoryIdResolver implements Resolve<IAssessmentQBank> {
 
     constructor(private qBankService: HrService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IAssessmentQBank> {
        return this.qBankService.getQBankByCategoryId(+route.paramMap.get('id'));
     }
 
 }