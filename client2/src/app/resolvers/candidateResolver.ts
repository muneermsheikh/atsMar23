import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { ICandidate } from "../shared/models/hr/candidate";
import { CandidateService } from "../candidates/candidate.service";

@Injectable({
     providedIn: 'root'
 })
 export class CandidateResolver implements Resolve<ICandidate|null> {
 
     constructor(private candidateService: CandidateService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICandidate|null> {

        var routeid = route.paramMap.get('id');
        
        if (routeid === '0' || routeid === null) return of(null);

        return this.candidateService.getCandidate(+routeid);
     }
 
 }