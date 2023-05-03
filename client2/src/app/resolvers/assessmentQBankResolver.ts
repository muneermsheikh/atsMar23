import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IAssessmentQBank } from "../shared/models/admin/assessmentQBank";
import { HrService } from "../hr/hr.service";


@Injectable({
     providedIn: 'root'
 })
 export class AssessmentQBankResolver implements Resolve<IAssessmentQBank[]> {
 
     constructor(private qBankService: HrService) {}
 
     resolve(): Observable<IAssessmentQBank[]> {
        return this.qBankService.getAssessmentQBank();
     }
 
 }