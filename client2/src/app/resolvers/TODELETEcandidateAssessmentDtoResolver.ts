import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CvAssessService } from "../hr/cv-assess.service";
import { IAssessmentsOfACandidateIdDto } from "../shared/dtos/assessmentsOfACandidateIdDto";


@Injectable({
     providedIn: 'root'
 })
 export class CandidateAssessmentDtoResolver implements Resolve<IAssessmentsOfACandidateIdDto[]> {
 
     constructor(private candidateService: CvAssessService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IAssessmentsOfACandidateIdDto[]> {
    
        var routeid = route.paramMap.get('id');
        console.log('routeid in resolver is:', routeid);
        if (routeid === '0') return null;
        var ret = this.candidateService.getCVAssessmentsOfACandidate(+routeid);
        console.log('candidateassessmentresolver', ret);

        return ret;
     }
 
 }