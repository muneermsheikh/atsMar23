import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { InterviewService } from "../interview/interview.service";
import { IInterview } from "../shared/models/hr/interview";
import { IInterviewItem } from "../shared/models/hr/interviewItem";
import { IPaginationInterview } from "../shared/pagination/paginationInterview";
import { candidatesMatchingParams } from "../shared/params/candidatesMatchingParams";

@Injectable({
     providedIn: 'root'
 })
 export class InterviewItemResolver implements Resolve<IInterviewItem> {
 
     constructor(private interviewService: InterviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IInterviewItem> {
        var interviewitemid = +route.paramMap.get('interviewitemid')
        var params = new candidatesMatchingParams();
        params.interviewId = +interviewitemid;
        this.interviewService.setParams(params);

        return this.interviewService.getInterviewItem(+interviewitemid);
     }
 
 }