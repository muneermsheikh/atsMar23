import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CandidateHistoryService } from "../candidate/candidate-history.service";
import { IUserHistory } from "../shared/models/admin/userHistory";


@Injectable({
     providedIn: 'root'
 })
 export class CandidateHistoryFromHistoryIdResolver implements Resolve<IUserHistory> {
 
     constructor(private candidateHistoryService: CandidateHistoryService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IUserHistory> {
        var routeid = route.paramMap.get('id');
        if (routeid === '0') return null;
        return this.candidateHistoryService.getCandidateHistoryByHistoryId(+routeid);
     }
 
 }