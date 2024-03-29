import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { InterviewService } from "src/app/interview/interview.service";
import { IInterview } from "src/app/shared/models/hr/interview";
//import { InterviewService } from "../interview/interview.service";
//import { IInterview } from "../shared/models/hr/interview";

@Injectable({
     providedIn: 'root'
 })
 export class InterviewFromOrderNoResolver implements Resolve<IInterview> {
 
     constructor(private interviewService: InterviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IInterview> {
        //if it exists in DB, returns Interview Object; else creates just an Object and returns it
        //this object then is meant to be edited and saved back to the DB
        return this.interviewService.getOrCreateInterviewFromOrderNo(+route.paramMap.get('orderno')!);
     }
 
 }