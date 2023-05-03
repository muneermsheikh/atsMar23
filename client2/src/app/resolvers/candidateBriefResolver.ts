import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { CandidateService } from "../candidates/candidate.service";
import { ICandidateBriefDto } from "../shared/dtos/admin/candidateBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class CandidateBriefResolver implements Resolve<ICandidateBriefDto|null> {
 
     constructor(private candidateService: CandidateService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICandidateBriefDto|null> {

        var routeid = route.paramMap.get('id');
        
        if (routeid === '0' || routeid === null) return of(null);

        return this.candidateService.getCandidateBrief(+routeid);
     }
 
 }