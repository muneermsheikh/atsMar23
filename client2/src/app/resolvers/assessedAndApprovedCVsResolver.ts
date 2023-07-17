import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";

import { CvrefService } from "../admin/cvref.service";
import { ICandidateAssessedDto } from "../shared/dtos/hr/candidateAssessedDto";
import { IPagination } from "../shared/models/pagination";


@Injectable({
     providedIn: 'root'
 })
 export class AssessedAndApprovedCVsResolver implements Resolve<IPagination<ICandidateAssessedDto[]>> {
 
     constructor(private cvrefService: CvrefService ) {}
 
     resolve(): Observable<IPagination<ICandidateAssessedDto[]>> {
        return this.cvrefService.getShortlistedCandidates(false);
     }
 
 }