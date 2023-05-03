import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { IUserHistory } from "../shared/models/admin/userHistory";
import { CandidateHistoryService } from "../candidates/candidate-history.service";
import { userHistoryParams } from "../shared/params/admin/userHistoryParams";


@Injectable({
     providedIn: 'root'
 })
 export class CandidateHistoryResolver implements Resolve<IUserHistory|null> {
 
     constructor(private candidateHistoryService: CandidateHistoryService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IUserHistory|null> {

        var hParam = new userHistoryParams();
        var routeid = route.paramMap.get('id');
        if ( routeid!=='' && routeid !== null && routeid !== '0') {
            hParam.personType='prospective';
            hParam.id = +routeid;
        } else {
            if (routeid !== '' && routeid !== '0' && routeid !== null) {
                hParam.personType='prospective';
                hParam.id=+routeid;
            } else {
                var officialId = route.paramMap.get('officialId');
                if(officialId !== null) {
                    hParam.personType='official';
                    hParam.personId = +officialId;
                } else {
                    return of(null);
                }
            }
        }
        
        //console.log('in cadiatehistoryresolver, hParam is:', hParam);
        return this.candidateHistoryService.getHistory(hParam);
            
        //return this.candidateHistoryService.getCandidateHistoryByHistoryId(+routeid);

        //return this.candidateHistoryService.getCandidateHistoryByCandidateId(+routeid);
     }
 
 }