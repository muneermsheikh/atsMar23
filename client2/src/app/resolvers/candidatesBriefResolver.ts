import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { ICandidateBriefDto } from "../shared/dtos/admin/candidateBriefDto";
import { CandidateService } from "../candidates/candidate.service";
import { IPagination } from "../shared/models/pagination";
import { paramsCandidate } from "../shared/params/hr/paramsCandidate";


@Injectable({
     providedIn: 'root'
 })
 export class CandidatesBriefResolver implements Resolve<IPagination<ICandidateBriefDto[]>> {
 
     constructor(private candidateService: CandidateService) {}
 
     resolve(): Observable<IPagination<ICandidateBriefDto[]>> {
        this.candidateService.setCVParams(new paramsCandidate());
        return this.candidateService.getCandidates(false);
     }
 
 }