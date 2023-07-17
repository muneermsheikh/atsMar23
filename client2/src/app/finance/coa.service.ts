import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { coaParams } from '../shared/params/finance/coaParams';
import { coaParamsFind } from '../shared/params/finance/coaParamsFind';
import { ICOA } from '../shared/models/masters/finance/coa';
import { IPagination } from '../shared/models/pagination';
import { CandidateCOAParams } from '../shared/dtos/finance/candidateCOAParams';
import { HttpClient, HttpParams } from '@angular/common/http';

import { map } from 'rxjs/operators';
import { coaDto } from '../shared/dtos/finance/coaDto';
import { IPendingDebitApprovalDto } from '../shared/dtos/finance/pendingDebitApprovalDto';

@Injectable({
  providedIn: 'root'
})
export class CoaService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new coaParams();
  candidatearams = new coaParamsFind();

  coalists: ICOA[]=[];
  pagination?: IPagination<ICOA[]>;  // = new PaginationCOA;
  cache = new Map();
  candidateCOAParams = new CandidateCOAParams();

  constructor(private http: HttpClient) { }

  getCoaList() {
    return this.http.get<ICOA[]>(this.apiUrl + 'finance/coaslist');
  }

  createCandidateCOA(appno: number) {
    return this.http.get<ICOA>(this.apiUrl + 'finance/coaforCandidate/' + appno + '/' + true);
  }

  getGroupOfCOAs(group: string) {
    return this.http.get<ICOA[]>(this.apiUrl + 'Finance/coabygroup/'+  group);
  }

  getCandidateCOAs(appno: number) {
    return this.http.get<ICOA[]>(this.apiUrl + 'Finance/coasforpayment/' + appno);
  }

  getDebitApprovalsPending() {
    return this.http.get<IPendingDebitApprovalDto[]>(this.apiUrl + 'finance/debitapprovalspending');
  }
  
  getCandidateCOAWithClBal(appno: number) {
    return this.http.get<coaDto>(this.apiUrl + 'finance/coaforcandidate/' + appno + '/' + false);
  }

  getCoas(useCache: boolean) {

    if (useCache === false) this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.sParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.sParams).join('-'));
        return of(this.pagination);
      }
    }

    let params = new HttpParams();

    if (this.sParams.accountName !== '' )  params = params.append('coaId', this.sParams.accountName);
    if (this.sParams.sort !== '') params = params.append('sort', this.sParams.sort);
    
    if (this.sParams.search) params = params.append('search', this.sParams.search);

    params = params.append('sort', this.sParams.sort);
    params = params.append('pageIndex', this.sParams.pageNumber.toString());
    params = params.append('pageSize', this.sParams.pageSize.toString());

    console.log('params:', params);
    return this.http.get<IPagination<ICOA[]>>(this.apiUrl + 'finance/coas', {params})
      .pipe(
        map((response: any) => {
          this.cache.set(Object.values(this.sParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
    }

  editCOA(coa : ICOA)
  {
      return this.http.put<ICOA>(this.apiUrl + 'finance/coa', coa);
  }

  addNewCOA(coa: any) {
    console.log('sending to api, coa:', coa);
    return this.http.post<ICOA>(this.apiUrl + 'finance/coa', coa);
  }
  
  deleteCOA(id: number)
  {
    return this.http.delete<boolean>(this.apiUrl + 'finance/coa/' + id);
  }

  updateCOA(coa:ICOA) {
    return this.http.put<boolean>(this.apiUrl + 'finance/coa', coa);
  }
  getMatchingCOAs(coaname: string) {
    return this.http.get<string[]>(this.apiUrl + 'finance/matchingaccountnames/' + coaname);
  }
    
  deleteFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.count--;
  }

  setParams(params: coaParams) {
    this.sParams = params;
  }
  
  getParams() {
    return this.sParams;
  }
}
