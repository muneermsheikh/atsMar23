import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { Observable, ReplaySubject, of } from 'rxjs';
import { SelDecisionParams } from '../shared/params/admin/selDecisionParams';
import { ISelPendingDto } from '../shared/dtos/admin/selPendingDto';
import { IPagination } from '../shared/models/pagination';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AccountService } from '../account/account.service';
import { map, take } from 'rxjs/operators';
import { selDecisionsToAddParams } from '../shared/params/admin/selDecisionsToAddParams';
import { ISelectionMsgsAndEmploymentsDto } from '../shared/dtos/admin/selectionMsgsAndEmploymentsDto';
import { ISelectionDecision } from '../shared/models/admin/selectionDecision';
import { ISelectionStatus } from '../shared/models/admin/selectionStatus';
import { IEmployment } from '../shared/models/admin/employment';

@Injectable({
  providedIn: 'root'
})
export class SelectionService {

  apiUrl = environment.apiUrl;
  user?: IUser;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new SelDecisionParams();

  pagination?: IPagination<ISelPendingDto[]>;  // = new paginationSelPending();

  cache = new Map();

  constructor(private activatedRoute: ActivatedRoute,
      private http: HttpClient, 
      private accountService: AccountService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
          this.user = user!;
        })
  }

  setParams(params: SelDecisionParams) {
    this.sParams = params;
   }

   getParams(): SelDecisionParams {
     return this.sParams;
   }

  private getHttpParams(): HttpParams {
    
    let params = new HttpParams();

    if (this.sParams.orderItemId !== 0) 
      params = params.append('orderItemId', this.sParams.orderItemId.toString());
    
    if (this.sParams.categoryId !== 0) 
      params = params.append('categoryId', this.sParams.categoryId.toString());
    if (this.sParams.categoryName !== '') 
      params = params.append('categoryName', this.sParams.categoryName);
    if (this.sParams.orderId !== 0) 
      params = params.append('orderId', this.sParams.orderId!.toString());
    if (this.sParams.orderNo !== 0) 
      params = params.append('orderNo', this.sParams.orderNo!.toString());
    if (this.sParams.candidateId !== 0) 
      params = params.append('candidateId', this.sParams.candidateId!.toString());
    if (this.sParams.applicationNo !== 0) 
      params = params.append('applicationNo', this.sParams.applicationNo!.toString());
    if (this.sParams.cVRefId !== 0) 
      params = params.append('cVRefId', this.sParams.cVRefId!.toString());
    if (this.sParams.includeEmployment === true) 
      params = params.append('includeEmployment', "true");
    if (this.sParams.search) 
      params = params.append('search', this.sParams.search);
    
    params = params.append('sort', this.sParams.sort);
    params = params.append('pageIndex', this.sParams.pageIndex.toString());
    params = params.append('pageSize', this.sParams.pageSize.toString());
    
    return params;
  }

  getPendingSelections(useCache: boolean): Observable<IPagination<ISelPendingDto[]>> {

    if (useCache === false) this.cache = new Map();

    if (this.cache.size > 0 && useCache) {
      if (this.cache.has(Object.values(this.sParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.sParams).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }
    
    var params = this.getHttpParams();

    return this.http.get<IPagination<ISelPendingDto[]>>(this.apiUrl + 
      'selectionDecision/pendingseldecisions', {params})
    .pipe(
      map(response => {
        this.cache.set(Object.values(this.sParams).join('-'), response);
        this.pagination = response!;
        return response;
      })
    )
    
    
  }


  registerSelectionDecisions(selDecisions: selDecisionsToAddParams) {
    return this.http.post<ISelectionMsgsAndEmploymentsDto>(this.apiUrl + 'selectiondecision', selDecisions);
  }

  getSelectionDecisions(selDecision: SelDecisionParams)
  {
       var params=this.getHttpParams();

      return this.http.get<ISelectionDecision[]>(this.apiUrl + 'seldecision', {observe: 'response', params})
  }

  editSelectionDecision(seldecision: ISelectionDecision) {
    return this.http.put<boolean>(this.apiUrl + 'selectiondecision', seldecision);
  }

  deleteSelectionDecision(id: number) {
    
    return this.http.delete<boolean>(this.apiUrl + 'selectiondecision/' + id);
  }

  getSelectionStatus() {
    return this.http.get<ISelectionStatus[]>(this.apiUrl + 'selectiondecision/selectionstatus');
  }

  getEmployment(cvrefid: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selectiondecision/employment/' + cvrefid);
  }

  getEmploymentFromSelectionId(id: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selectiondecision/employmentfromSelId/' + id);
    
  }

  updateEmployment(emp: IEmployment) {
    return this.http.put<boolean>(this.apiUrl + 'selectiondecision/employment', emp);
  }

  getSelectionDtoByOrderNo(orderno: number) {
    
  }
}
