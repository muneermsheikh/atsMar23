import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { InterviewService } from "../interview/interview.service";
import { ICandidateBriefDto } from "../shared/models/candidateBriefDto";
import { IInterview } from "../shared/models/hr/interview";

@Injectable({
     providedIn: 'root'
 })
 export class interviewItemMatchingCandidatesdResolver implements Resolve<ICandidateBriefDto[]> {
 
     constructor(private interviewService: InterviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICandidateBriefDto[]> {
        var interviewid = +route.paramMap.get('interviewitemid')
        return this.interviewService.getCandidatesMatchingInterviewCategoryId(+interviewid);
     }
 
 }