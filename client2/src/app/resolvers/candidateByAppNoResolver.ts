import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CandidateService } from "../candidate/candidate.service";
import { ICandidate } from "../shared/models/candidate";


@Injectable({
     providedIn: 'root'
 })
 export class CandidateByAppNoResolver implements Resolve<ICandidate> {
 
     constructor(private candidateService: CandidateService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICandidate> {

        var routeid = route.paramMap.get('appno');
        if (routeid === '0') return null;
        return this.candidateService.getCandidatebyappno(+routeid);
     }
 
 }