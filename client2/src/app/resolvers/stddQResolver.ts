import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { StddqsService } from "../hr/stddqs.service";
import { IAssessmentStandardQ } from "../shared/models/assessmentStandardQ";


@Injectable({
     providedIn: 'root'
 })
 export class StddQResolver implements Resolve<IAssessmentStandardQ> {
 
     constructor(private service: StddqsService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IAssessmentStandardQ> {
        var routeid = route.paramMap.get('id');
        if (routeid === '') return null;
        return this.service.getStddQ(+routeid);
     }
 
 }