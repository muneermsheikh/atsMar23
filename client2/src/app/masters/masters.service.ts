import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from 'src/environments/environment';
import { paramsMasters } from '../shared/params/masters/paramsMasters';
import { IPagination } from '../shared/models/pagination';
import { IIndustryType, IProfession } from '../shared/models/masters/profession';
import { IUser } from '../shared/models/admin/user';
import { ICustomerNameAndCity } from '../shared/models/admin/customernameandcity';
import { IEmployeeIdAndKnownAs } from '../shared/models/admin/employeeIdAndKnownAs';
import { IQualification } from '../shared/models/hr/qualification';
import { IVendorFacility } from '../shared/models/admin/vendorFacility';

@Injectable({
  providedIn: 'root'
})
export class MastersService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new paramsMasters();
  empParams = new paramsMasters();
  empKnownAs?: IEmployeeIdAndKnownAs;
  empsKnownAs: IEmployeeIdAndKnownAs[]=[];

  paginationCategory?: IPagination<IProfession[]>;
  paginationQualification?: IPagination<IQualification[]>;
  paginationIndsTypes?: IPagination<IIndustryType[]>;

  professions: IProfession[]=[];
  
  agents: ICustomerNameAndCity[]=[];
  customers: ICustomerNameAndCity[]=[];

  cacheCat = new Map();
  cacheQ = new Map();   //qualifications
  cacheEmp = new Map();
  cacheInd = new Map();

  constructor(private http: HttpClient) { }
  
  getCategoryList() {

      return this.http.get<IProfession[]>(this.apiUrl + 'masters/categories', {});
  }

  getVendorFacilityList() {
    return this.http.get<IVendorFacility[]>(this.apiUrl + 'masters/VendorFacilityList')
  }

  getCategories(useCache: boolean) { 

    if (useCache === false)  this.cacheCat = new Map();
    
    if (this.cacheCat.size > 0 && useCache === true) {
      if (this.cacheCat.has(Object.values(this.mParams).join('-'))) {
        this.paginationCategory!.data = this.cacheCat.get(Object.values(this.mParams).join('-'));
        return of(this.paginationCategory);
      }
    }

    let params = new HttpParams();
    if (this.mParams.name !== '') params = params.append('name', this.mParams.name!);
    if (this.mParams.id !== 0) params = params.append('id', this.mParams.id!.toString());
    if (this.mParams.search) params = params.append('search', this.mParams.search);
      
    params = params.append('pageIndex', this.mParams.pageNumber.toString());
    params = params.append('pageSize', this.mParams.pageSize.toString());
      
    return this.http.get<IPagination<IProfession[]>>(this.apiUrl + 'masters/cpaginated', 
        {observe: 'response', params})
        .pipe(
          map(
            (response) => {
              this.cacheCat.set(Object.values(this.mParams).join('-'), response.body?.data);
              this.paginationCategory = response.body!;
              return response.body!;
          })
        )
      
  }

  getQualificationList(){
    console.log('calling api for qlist');
    return this.http.get<IQualification[]>(this.apiUrl + 'masters/qualificationList');
  }

  getCategory(id: number) {
    let category: IProfession;
    this.cacheCat.forEach((categories: IProfession[]) => {
      //console.log(product);
      category = categories.find(p => p.id === id)!;
    })

    if (category!) {
      return of(category);
    }

    return this.http.get<IProfession>(this.apiUrl + 'masters/category/' + id);
  }

  deleteCategory(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'masters/deletecategory/' + id);
  }

//quaifications
  getQualifications(useCache: boolean): Observable<IPagination<IQualification[]> | null|undefined> { 

    if (useCache === false)  this.cacheQ = new Map();
    
    if (this.cacheQ.size > 0 && useCache === true) {
      if (this.cacheQ.has(Object.values(this.mParams).join('-'))) {
        this.paginationQualification!.data = this.cacheQ.get(Object.values(this.mParams).join('-'));
        return of(this.paginationQualification);
      }
    }

    let params = new HttpParams();
    if (this.mParams.name !== '') params = params.append('name', this.mParams.name!);
    if (this.mParams.id !== 0) params = params.append('id', this.mParams.id!.toString());
    if (this.mParams.search) params = params.append('search', this.mParams.search);
      
    params = params.append('pageIndex', this.mParams.pageNumber.toString());
    params = params.append('pageSize', this.mParams.pageSize.toString());
      
    return this.http.get<IPagination<IQualification[]>>(this.apiUrl + 'masters/qpaginated', 
        {observe: 'response', params})
        .pipe(
          map(
            response => {
              this.cacheQ.set(Object.values(this.mParams).join('-'), response.body?.data);
              //console.log('masters.service, response returned is:', response);
              this.paginationQualification = response.body!;
              return response.body;
          })
        )
      
  }

  getQualification(id: number) {
    let qualification: IQualification;
    this.cacheCat.forEach((qs: IQualification[]) => {
      //console.log(product);
      qualification = qs.find(p => p.id === id)!;
    })

    if (qualification!) {
      return of(qualification);
    }

    return this.http.get<IQualification>(this.apiUrl + 'masters/qualification/' + id);
  }

  updateQualification(id: number, name: string) {
    var prof: IQualification = {id: id, name: name};
    return this.http.put<IQualification>(this.apiUrl + 'masters/editqualification', prof);
  }

  deleteQualification(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'masters/deletequalification/' + id);
  }
  
//industries

  getIndustryPaged(useCache: boolean) {
    if (useCache === false)  this.cacheInd = new Map();
    
    if (this.cacheInd.size > 0 && useCache === true) {
      if (this.cacheInd.has(Object.values(this.mParams).join('-'))) {
        this.paginationIndsTypes!.data = this.cacheInd.get(Object.values(this.mParams).join('-'));
        return of(this.paginationIndsTypes);
      }
    }

    let params = new HttpParams();
    if (this.mParams.name !== '') params = params.append('name', this.mParams.name!);
    if (this.mParams.id !== 0) params = params.append('id', this.mParams.id!.toString());
    if (this.mParams.search) params = params.append('search', this.mParams.search);
      
    params = params.append('pageIndex', this.mParams.pageNumber.toString());
    params = params.append('pageSize', this.mParams.pageSize.toString());
      
    return this.http.get<IPagination<IIndustryType[]>>(this.apiUrl + 'masters/indpaginated', 
        {observe: 'response', params})
        .pipe(
          map(
            (response) => {
              this.cacheInd.set(Object.values(this.mParams).join('-'), response.body?.data);
              this.paginationIndsTypes = response.body!;
              return response.body!;
          })
        )
    }

  getIndustry(id:number) {
    return this.http.get<IIndustryType>(this.apiUrl + 'masters/industry')
  }

  getIndustries() {
    return this.http.get<IIndustryType[]>(this.apiUrl + 'masters/industrieslist');
  }

  deleteIndustry(industryid: number) {
    return this.http.delete<boolean>(this.apiUrl+ 'masters/deleteindustry/' +  industryid);
  }

  updateCategory(id: number, name: string) {
    var prof: IProfession = {id: id, name: name};
    return this.http.put<IProfession>(this.apiUrl + 'masters/editcategory', prof);
  }
  
  updateIndustry(id: number, name: string) {
    var ind: IIndustryType = {id: id, name: name};
    return this.http.put<boolean>(this.apiUrl + 'masters/editindustry', ind);
  }
  
  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.apiUrl + 'employees/idandknownas');
  }


  setParams(params: paramsMasters) {
    this.mParams = params;
  }
  
  getParams() {
    return this.mParams;
  }

  getAgents() {
    if (this.agents.length > 0) {
      return of(this.agents);
    }
    
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/idandnames/associate').pipe(
      map(response => {
        this.agents = response;
        return response;
      })
    );
  }
}
