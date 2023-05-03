import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { assessmentStddQParam } from '../shared/models/admin/assessmentStddQParam';
import { IAssessmentStandardQ } from '../shared/models/admin/assessmentStandardQ';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account/account.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class StddqsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new assessmentStddQParam();
  stddqs: IAssessmentStandardQ[]=[];
  stddq?: IAssessmentStandardQ;

  routeId: string='';
  user?: IUser;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getStddQsWithoutCache() {
    return this.http.get<IAssessmentStandardQ[]>(this.apiUrl + 'assessmentstddq')
    .pipe(
      map(response => {
        this.cache.set(Object.values(this.qParams).join('-'), response);
        this.stddqs = response;
        return response;
      })
    );
  }

  getStddQs(useCache: boolean=false) {
    if (useCache === false) this.cache = new Map();

    if(this.cache.size > 0 && useCache === true) {
      if(this.cache.has(Object.values(this.qParams).join('-'))) {
        this.stddqs=this.cache.get(Object.values(this.qParams).join('-'));
        return of(this.stddqs);
      }
    }

    let params = new HttpParams();
    if(this.qParams.id !== 0) {
      params = params.append('id', this.qParams.id!.toString());
    }
    if(this.qParams.parameter !== '') {
      params = params.append('parameter', this.qParams.parameter);
    }
    if (this.qParams.search) {
      params = params.append('search', this.qParams.search);
    }

    return this.http.get<IAssessmentStandardQ[]>(this.apiUrl + 'assessmentstddq', {observe: 'response', params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(this.qParams).join('-'), response.body);
          this.stddqs = response.body!;

          return response.body;
        })
      )
  }

  getStddQ(id: number) {
    if(this.cache.size > 0) {
      const qparam = new assessmentStddQParam();
      qparam.id=id;
      if(this.cache.has(Object.values(qparam).join('-'))) {
        this.stddq=this.cache.get(Object.values(qparam).join('-'));
        console.log('retrieved stdQ from cache');
        return of(this.stddq);
      }
    }
    console.log('retrieving stddQ from api');
    return this.http.get<IAssessmentStandardQ>(this.apiUrl + 'assessmentstddq/byid/' + id);
  }

  deletestddq(id: number) {
    return this.http.delete(this.apiUrl + 'assessmentstddq/' + id);
  }

  createStddQ(q:IAssessmentStandardQ) {
    return this.http.post<IAssessmentStandardQ>(this.apiUrl + 'assessmentstddq', q);
  }

  updateStddQ(qs:IAssessmentStandardQ[]) {
    return this.http.put<boolean>(this.apiUrl + 'assessmentstddq', qs);
  }
  
  setQParams(params: assessmentStddQParam) {
    this.qParams = params;
  }
  getQParams() {
    return this.qParams;
  }

}
