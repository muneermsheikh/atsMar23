import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { ReplaySubject, of } from 'rxjs';
import { IPagination } from '../shared/models/pagination';
import { CVRefParams } from '../shared/params/admin/cvRefParams';
import { ActivatedRoute } from '@angular/router';
import { AccountService } from '../account/account.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, take } from 'rxjs/operators';
import { IMessagesDto } from '../shared/dtos/admin/messagesDto';
import { ICandidateAssessedDto } from '../shared/dtos/hr/candidateAssessedDto';


@Injectable({
  providedIn: 'root'
})
export class CvrefService {

  apiUrl = environment.apiUrl;
  user?: IUser;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  pagination? : IPagination<ICandidateAssessedDto[]>;
  cache = new Map();

  cvRefParams = new CVRefParams();

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService,
      private http: HttpClient) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  getShortlistedCandidates(useCache: boolean) { 

    if (useCache === false)  this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.cvRefParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.cvRefParams).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }

    let params = new HttpParams();
    if (this.cvRefParams.agentId !== 0) params = params.append('agentId', this.cvRefParams.agentId!.toString());
    if (this.cvRefParams.professionId !== 0) params = params.append('professionId', this.cvRefParams.professionId!.toString());
    if (this.cvRefParams.applicationNo !== 0) params = params.append('agentId', this.cvRefParams.applicationNo!.toString());
    if (this.cvRefParams.candidateId !== 0) params = params.append('candidateId', this.cvRefParams.candidateId!.toString());
    
    if (this.cvRefParams.search) params = params.append('search', this.cvRefParams.search);
    
    params = params.append('sort', this.cvRefParams.sort);
    params = params.append('pageIndex', this.cvRefParams.pageNumber.toString());
    params = params.append('pageSize', this.cvRefParams.pageSize.toString());

    return this.http.get<IPagination<ICandidateAssessedDto[]>>(this.apiUrl + 
        'candidateassessment/shortlistedpaginated', {params}).pipe(
          map(response => {
            this.cache.set(Object.values(this.cvRefParams).join('-'), response);
            this.pagination = response;
            return response;  
          })
        )
    }

    setCVRefParams(params: CVRefParams) {
      this.cvRefParams = params;
    }
    getCVRefParams() {
      return this.cvRefParams;
    }
  

  referCVs(cvassessmentids: number[]) {
    return this.http.post<IMessagesDto>(this.apiUrl + 'CVRef', cvassessmentids);
  }
}
