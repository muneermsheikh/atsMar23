import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { AssessmentService } from "../hr/assessment.service";
import { IAssessment } from "../shared/models/admin/assessment";
import { NullTemplateVisitor } from "@angular/compiler";


@Injectable({
     providedIn: 'root'
 })
 export class AssessmentQsResolver implements Resolve<IAssessment|null> {
 
     constructor(private service: AssessmentService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IAssessment|null> {

        var id = route.paramMap.get('id');
        if(id===null) return of(null);
        
        var assessmt = this.service.getOrderItemAssessment(+id);

        return assessmt;
     }
 
 }