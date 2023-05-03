import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CandidateBriefDto, ICandidateBriefDto } from '../shared/dtos/admin/candidateBriefDto';
import { CVBriefParam } from '../shared/params/hr/cvBriefParam';
import { IPagination } from '../shared/models/pagination';
import { AccountService } from '../account/account.service';
import { take } from 'rxjs/operators';
import { IOrderItemAssessmentQ } from '../shared/models/admin/orderItemAssessmentQ';
import { ICandidateAssessment } from '../shared/models/hr/candidateAssessment';
import { ICandidateAssessmentWithErrorStringDto } from '../shared/dtos/hr/candidateAssessmentWithErrorStringDto';
import { ICandidateAssessmentAndChecklist } from '../shared/models/hr/candidateAssessmentAndChecklist';
import { ICandidateAssessedDto } from '../shared/dtos/hr/candidateAssessedDto';

@Injectable({
  providedIn: 'root'
})
export class CvAssessService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  user?: IUser;
  header?: HttpHeaders;

  cvBriefs: ICandidateBriefDto[]=[];

  cache = new Map();
  cvParams = new CVBriefParam();

  pagination?: IPagination<CandidateBriefDto>; // = new PaginationCandidateBrief();

  constructor(private http: HttpClient, private accountService: AccountService
    ) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

 
  updateCVAssessment(model: any) {
    return this.http.put(this.apiUrl + 'candidateassessment/assess', model);
  }

  getOrderItemAssessmentQs(orderitemid: number) {
    return this.http.get<IOrderItemAssessmentQ[]>(this.apiUrl + 'orderassessment/itemassessmentQ/' + orderitemid);
  }

  updateCVAssessmentHeader(model: ICandidateAssessment) {
    return this.http.put<ICandidateAssessmentWithErrorStringDto>(this.apiUrl + 'candidateassessment', model);
  }

  insertCVAssessmentHeader(requireReview: boolean, candidateid: number, orderitemid: number, dt: Date) {
    
    return this.http.post<ICandidateAssessmentWithErrorStringDto>(this.apiUrl + 'candidateassessment/assess/' 
      +  requireReview + '/' + candidateid + '/' + orderitemid, {});
  }

  getCVAssessmentObject(requireReview: boolean, candidateid: number, orderitemid: number, dt: Date) {
    return this.http.get<ICandidateAssessment>(this.apiUrl + 'candidateassessment/assessobject/' +  requireReview + '/' + candidateid + '/' + orderitemid);
  }


  insertCVAssessment(model: any) {
    return this.http.post(this.apiUrl + 'candidateassessment/assess', model);
  }

  getCVAssessment(cvid: number, orderitemid: number) {
    return this.http.get<ICandidateAssessment>(this.apiUrl + 'candidateassessment/' + orderitemid + '/' + cvid);
  }

  getCVAssessmentAndChecklist(cvid: number, orderitemid: number) {
    return this.http.get<ICandidateAssessmentAndChecklist>(this.apiUrl + 'candidateassessment/assessmentandchecklist/' + orderitemid + '/' + cvid);
  }


  getCVAssessmentsOfACandidate(cvid: number) {
    return this.http.get<ICandidateAssessedDto[]>(this.apiUrl + 'candidateassessment/assessmentsofcandidateid/' +cvid);
  }


  deleteAssessment(assessmentid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'candidateassessment/assess/' + assessmentid);
  }

  setCVParams(params: CVBriefParam) {
    this.cvParams = params;
  }

  getCVParams() {
    return this.cvParams;
  }

  getCVBrief() { 
    
    var brief = this.cvBriefs.filter(x => x.id===this.cvParams.candidateId)[0];
    //console.log('in cvassess serice, cvparams.candidateid', this.cvParams.candidateId,'brief:', brief);
    return brief;

    }

    setCVBriefData(cvbriefs: ICandidateBriefDto[]) {
      this.cvBriefs = cvbriefs;
      this.cache.set(Object.values(this.cvParams).join('-'), cvbriefs);
      this.pagination!.count=this.cvBriefs.length;
    }
  
    getCVBriefData() {
      if (this.cache.has(Object.values(this.cvParams).join('-'))) {
        this.pagination!.data = this.cache.get(Object.values(this.cvParams).join('-'));
        return of(this.pagination);
      }
      return of(undefined);
    }
  

  getCVBriefs(useCache: boolean) { 

      if (useCache === false)  this.cache = new Map();

      if (this.cache.size > 0 && useCache === true) {
        if (this.cache.has(Object.values(this.cvParams).join('-'))) {
          this.pagination!.data = this.cache.get(Object.values(this.cvParams).join('-'));
          return of(this.pagination);
        }
      }
     return of(undefined);
  }
  
}
