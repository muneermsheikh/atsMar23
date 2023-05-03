import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { InterviewService } from "../interview/interview.service";
import { IPaginationInterview } from "../shared/pagination/paginationInterview";

@Injectable({
     providedIn: 'root'
 })
 export class InterviewsBriefResolver implements Resolve<IPaginationInterview> {
 
     constructor(private interviewService: InterviewService) {}
 
     resolve(): Observable<IPaginationInterview> {
        console.log('etered interviewsbriefresolver');
        return this.interviewService.getInterviews(false);
     }
 
 }