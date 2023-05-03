import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CvRefService } from "../admin/cvref.service";
import { ICandidateAssessedDto } from "../shared/models/candidateAssessedDto";


@Injectable({
     providedIn: 'root'
 })
 export class AssessedAndApprovedCVsResolver implements Resolve<ICandidateAssessedDto[]> {
 
     constructor(private cvrefService: CvRefService ) {}
 
     resolve(): Observable<ICandidateAssessedDto[]> {
        return this.cvrefService.getCVsAssessedAndApproved();
     }
 
 }