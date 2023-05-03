import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { StddqsService } from "../hr/stddqs.service";
import { IAssessmentStandardQ } from "../shared/models/admin/assessmentStandardQ";

@Injectable({
     providedIn: 'root'
 })
 export class AssessmentStddQResolver implements Resolve<IAssessmentStandardQ|undefined> {
 
     constructor(private service: StddqsService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IAssessmentStandardQ|undefined> {

        var routeid = route.paramMap.get('id');
        if(routeid===null) return of(undefined);
        
        var q = this.service.getStddQ(+routeid);
        return q;
     }
 
 }