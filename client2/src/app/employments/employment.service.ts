import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { employmentParams } from '../shared/params/admin/employmentParam';
import { IPagination } from '../shared/models/pagination';
import { IEmployment } from '../shared/models/admin/employment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EmploymentService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  eParams = new employmentParams();
  pagination?: IPagination<IEmployment[]>;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getEmployments(useCache: boolean=true): Observable<IPagination<IEmployment[]>> { 

    if (useCache === false) this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.eParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.eParams).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }

    console.log('emp params:', this.eParams);

    let params = new HttpParams();
    if (this.eParams.cvRefId !== 0) 
      params = params.append('cvRefId', this.eParams.cvRefId!.toString());
    
    if (this.eParams.orderItemId !== 0) 
      params = params.append('orderItemId', this.eParams.orderItemId!.toString());
    
    if (this.eParams.categoryId !== 0) 
      params = params.append('categoryId', this.eParams.categoryId!.toString());
    
    if (this.eParams.orderId !== 0) 
      params = params.append('orderId', this.eParams.orderId!.toString());
    
    if (this.eParams.orderNo !== 0) 
      params = params.append('orderNo', this.eParams.orderNo!.toString());
  
    if (this.eParams.customerId !== 0) 
      params = params.append('customerId', this.eParams.customerId!.toString());
    
    if(this.eParams.applicationNo === 0) {
      params = params.append('applicationNo', "1");   //bcz the parameters in api is otherwise null/blanks
    } else {
      params = params.append('applicationNo', this.eParams.applicationNo!.toString());
    }

    
    if (this.eParams.candidateName !== '') 
      params = params.append('candidateName', this.eParams.candidateName);
    
    params = params.append('approved', this.eParams.approved );
    
    if (this.eParams.selectionDateFrom.getFullYear() > 2000) 
      params = params.append('selectionDateFrom', this.eParams.selectionDateFrom.toString());
    
    if (this.eParams.selectionDateUpto.getFullYear() > 2000) 
      params = params.append('selectionDateUpto', this.eParams.selectionDateUpto.toString());
    
    
    if (this.eParams.search) 
      params = params.append('search', this.eParams.search);
    
    params = params.append('sort', this.eParams.sort);
    params = params.append('pageIndex', this.eParams.pageNumber.toString());
    params = params.append('pageSize', this.eParams.pageSize.toString());

    console.log('httpParams sent to api:', params);
    return this.http.get<IPagination<IEmployment[]>>(this.apiUrl + 'employment', {params}).pipe(
        map(response => {
          this.cache.set(Object.values(this.eParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
    }

  updateEmployment(model: any) {
    return this.http.put(this.apiUrl + 'employment/employment', model);
  }

  
  setEParams(params: employmentParams) {
    this.eParams = params;
  }
  
  getEParams() {
    return this.eParams;
  }

  deleteEmployment(empid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'empid')
  }

}

