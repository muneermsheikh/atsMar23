import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { InterviewService } from "../interview/interview.service";
import { IInterviewAttendanceDto } from "../shared/dtos/interviewAttendanceDto";

@Injectable({
     providedIn: 'root'
 })
 export class InterviewAttendanceResolver implements Resolve<IInterviewAttendanceDto> {
 
     constructor(private interviewService: InterviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IInterviewAttendanceDto> {
        var orderid = +route.paramMap.get('orderid')

        return this.interviewService.interviewAttendance(+orderid);
     }
 
 }