import { Injectable } from '@angular/core';
import {Resolve, ActivatedRouteSnapshot} from '@angular/router';
import { Observable, of } from 'rxjs';
import { CvAssessService } from '../hr/cv-assess.service';
import { ICandidateAssessedDto } from '../shared/dtos/hr/candidateAssessedDto';


@Injectable({
  providedIn: 'root'
})

export class CandidateAssessedResolver implements Resolve<ICandidateAssessedDto[]|null> {
  constructor(private candidateService: CvAssessService) {}
  
  resolve(route: ActivatedRouteSnapshot): Observable<ICandidateAssessedDto[]|null> {
    
    var routeid = route.paramMap.get('id');
    
    if(routeid===null) return of(null);

    var ret = this.candidateService.getCVAssessmentsOfACandidate(+routeid);
    
    return ret;
  }
}
