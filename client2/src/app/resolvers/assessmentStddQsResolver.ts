import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { StddqsService } from "../hr/stddqs.service";
import { IAssessmentStandardQ } from "../shared/models/admin/assessmentStandardQ";

@Injectable({
     providedIn: 'root'
 })
 
 export class AssessmentStddQsResolver implements Resolve<IAssessmentStandardQ[]> {
 
     constructor(private service: StddqsService) {}
 
     resolve(): Observable<IAssessmentStandardQ[]> {

        var q = this.service.getStddQsWithoutCache();
        
        return q;
     }
 
 }