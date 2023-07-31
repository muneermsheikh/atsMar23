import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { userHistoryParams } from '../shared/params/admin/userHistoryParams';
import { IPagination } from '../shared/models/pagination';
import { IUserHistoryDto } from '../shared/dtos/admin/userHistoryDto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { IUserHistory } from '../shared/models/admin/userHistory';
import { IUserHistoryItem } from '../shared/models/admin/userHistoryItem';

@Injectable({
  providedIn: 'root'
})
export class CallRecordsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  hParams = new userHistoryParams();
  pagination?: IPagination<IUserHistoryDto[]>;
  cache = new Map();
  
  constructor(private http: HttpClient) { }

  getHistories(useCache: boolean=true): Observable<IPagination<IUserHistoryDto[]>> { 

    if (useCache === false) this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.hParams).join('-'))) {
        this.pagination!.data = this.cache.get(Object.values(this.hParams).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }

    var params = this.getHttpParams();
    
    if(this.hParams.userName !== '') params = params.append('userName', this.hParams.userName);
    if(this.hParams.applicationNo !== undefined) params = params.append('applicationNo', this.hParams.applicationNo?.toString());
    if(this.hParams.emailId !== '') params = params.append('emailId', this.hParams.emailId);
    if(this.hParams.mobileNo !== '') params = params.append('mobileNo', this.hParams.mobileNo);
    if(this.hParams.personName !== '') params = params.append('personName', this.hParams.personName);
    if(this.hParams.status !== '') params = params.append('status', this.hParams.status);

    params = params.append('sort', this.hParams.sort);
    params = params.append('pageIndex', this.hParams.pageNumber.toString());
    params = params.append('pageSize', this.hParams.pageSize.toString());

    return this.http.get<IPagination<IUserHistoryDto[]>>(
        this.apiUrl + 'userHistory/paginated', {params}).pipe(
        map((response: IPagination<IUserHistoryDto[]>) => {
          this.cache.set(Object.values(this.hParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
    }

  getHistoryWithItems(historyId: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'userHistory/historywithitems/' + historyId);
  }

  updateHistoryItem(item: IUserHistoryItem) {
    return this.http.post<IUserHistoryItem>(this.apiUrl + 'userHistory/userhistoryitem', item);
  }
  
  getHttpParams(){
    let params = new HttpParams();

    if (this.hParams.personType !== "") 
      params = params.append('personType', this.hParams.personType);
    
    if (this.hParams.personName !== '') 
      params = params.append('personName', this.hParams.personName);
    
    if (this.hParams.applicationNo !== undefined ) 
      params = params.append('applicationNo', this.hParams.applicationNo?.toString());
    
    if (this.hParams.emailId !== '') 
      params = params.append('emailId', this.hParams.emailId);
    
    if (this.hParams.mobileNo !== '') 
      params = params.append('mobileNo', this.hParams.mobileNo);
    
    if (this.hParams.status !== '') 
      params = params.append('status', this.hParams.status);
    
    params = params.append('sort', this.hParams.sort);
    params = params.append('pageIndex', this.hParams.pageNumber.toString());
    params = params.append('pageSize', this.hParams.pageSize.toString());

    return params;
  }

  setParams(params: userHistoryParams) {
    this.hParams = params;
  }
  
  getParams() {
    return this.hParams;
  }

  updateHistory(model: any) {
    console.log('model in updatecustomer', model);
    return this.http.put(this.apiUrl + 'userHistory', model);
  }

  deleteHistory(historyid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'userHistory/' + historyid);
  }

  deleteHistoryItem(itemid: number) {
    return this.http.delete(this.apiUrl + 'userHistory/historyItemId/' + itemid);
  }
  getOrcreateNewHistory() {
    var params = this.getHttpParams();

    return this.http.get<IUserHistoryDto>(this.apiUrl + 'userHistory/dto', {params});
  }
}
