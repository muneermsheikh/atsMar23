import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CandidateHistoryService } from "../candidate/candidate-history.service";
import { IUserHistory } from "../shared/models/admin/userHistory";
import { userHistoryParams } from "../shared/params/userHistoryParams";


@Injectable({
     providedIn: 'root'
 })
 export class CandidateHistoryFromProspectiveIdResolver implements Resolve<IUserHistory> {
 
     constructor(private candidateHistoryService: CandidateHistoryService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IUserHistory> {

        var routeid = route.paramMap.get('id');
        if (routeid === '0') return null;
        var hParams = new userHistoryParams();
        hParams.personType="prospective";
        hParams.personId = +routeid;
        return this.candidateHistoryService.getHistory(hParams);
     }
 
 }