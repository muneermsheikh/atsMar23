import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { userHistoryParams } from '../shared/params/admin/userHistoryParams';
import { categoryRefParam } from '../shared/params/admin/categoryRefParam';
import { ICategoryRefDto } from '../shared/dtos/admin/categoryRefDto';
import { IPagination } from '../shared/models/pagination';
import { IUserHistory } from '../shared/models/admin/userHistory';
import { userHistoryHeaderParams } from '../shared/params/admin/userHistoryheaderParams';
import { IUserHistoryDto } from '../shared/dtos/admin/userHistoryDto';
import { ICandidateBriefDto } from '../shared/dtos/admin/candidateBriefDto';
import { IUserHistoryHeader } from '../shared/models/admin/userHistoryHeader';
import { userHistorySpecParams } from '../shared/params/admin/userHistorySpecParams';
import { IContactResult } from '../shared/models/admin/contactResult';
import { IUserHistoryReturnDto } from '../shared/dtos/admin/userHistoryReturnDto';
import { IUserHistoryItem } from '../shared/models/admin/userHistoryItem';


@Injectable({
  providedIn: 'root'
})
export class CandidateHistoryService {
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  histParams = new userHistoryParams();
  categoryRefParams = new categoryRefParam();
  categoryRefDtos: ICategoryRefDto[]=[];
  categoryRefDto?: ICategoryRefDto;

  cache = new Map();

  pagination?: IPagination<IUserHistory>;

  //for headers
  headerParams = new userHistoryHeaderParams();

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  setParams(params:categoryRefParam) {
    this.categoryRefParams = params;
  }
  
  getParams() {
    return this.categoryRefParams;
  }

  getCategoryRefDetailFromParam(useCache: boolean){
    if(!useCache) this.cache=new Map();

    if(this.cache.size > 0 && useCache) {
      if(this.cache.has(Object.values(this.categoryRefParams).join('-'))) {
        this.categoryRefDto = this.cache.get(Object.values(this.categoryRefParams).join('-'));
        return of(this.categoryRefDto);
      }
    }

    return of(undefined);

  }
  getCategoryRefDetails() {
    
    return this.http.get<ICategoryRefDto[]>(this.apiUrl + 'categoryrefdetails')
      .pipe(map(response => {
        this.cache.set(Object.values(this.categoryRefParams).join('-'), response);
        this.categoryRefDtos=response;
        return response;
      }))
  }

  getCandidateHistory(id: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/bycandidateid/'+id);
  }
  
  getHistory(hParams: userHistoryParams): any {
    //console.log('hParams in candidaatehistoryservice.getHistory', hParams);
    if(hParams.emailId==='' && hParams.id===0 && hParams.mobileNo==='' && hParams.personId===0 && hParams.personName==='' && hParams.applicationNo===0) {
      this.toastr.info('Params null');
      return null;
    }
    
    let params = new HttpParams();
    if (hParams.emailId !== "") params = params.append('emailId', hParams.emailId);
    if (hParams.id! > 0) params = params.append('id', hParams.id!.toString());
    if (hParams.mobileNo !== '' ) params = params.append('mobileNo', hParams.mobileNo);
    if (hParams.personId !== 0 ) params = params.append('personId', hParams.personId!.toString() );
    if (hParams.personName !== '' ) params = params.append('personName', hParams.personName);
    if (hParams.applicationNo! > 0) params = params.append('applicationNo', hParams.applicationNo!);
    if (hParams.personType !== '') {
      params.append('personType', hParams.personType);
    } else {
      hParams.personType="candidate";
    }
  
    params= params.append('personType', hParams.personType);
    params = params.append('createNewIfNull', hParams.createNewIfNull);

    return this.http.get<IUserHistoryDto>(this.apiUrl + 'userhistory/dto', {observe: 'response', params} ) ;
    
  }

  getHistoryFromCallerNamePhone(callername: string, mobileno: string): any {
    
    return this.http.get<ICandidateBriefDto>(this.apiUrl + 'userhistory/dtofromnameandphone/' + callername + '/' + mobileno);
    
  }

  getUserHistoryHeaders(hParams: userHistoryHeaderParams) {

    let params = new HttpParams();
    if (hParams.assignedToId !== 0) params = params.append('assignedToId', hParams.assignedToId!.toString());
    if (hParams.assignedById !== 0) params = params.append('assignedById', hParams.assignedById!.toString());
    if (hParams.status !== '' ) params = params.append('status', hParams.status!);
    if(hParams.status==='' && hParams.assignedToId===0 && hParams.assignedById===0) {
      this.toastr.info('Params null');
      return null;
    }

    return this.http.get<IPagination<IUserHistoryHeader>>(this.apiUrl + 'userHistory/headers', {observe: 'response', params});
  }
  
  getUserHistories(model: userHistoryParams) {
    var specParams: any = new userHistorySpecParams();
    specParams.aadharNo = model.aadharNo;
    specParams.applicationNo = model.applicationNo;
    specParams.emailId = model.emailId;
    specParams.customerOfficialId = model.customerOfficialId;
    specParams.createNewIfNull=false;
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/historyDto', specParams);
  }

  getCandidateHistoryByHistoryId(id: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/byhistoryid/' + id);
  }

  getCandidateHistoryByCandidateId(id: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/bycandidateid/' + id);
  }

  getUserHistoriesByPhoneNo(phoneno: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromphoneno/' + phoneno);
  }

  getUserHistoriesByOfficialId(officialid: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromofficialid/' + officialid);
  }

  getUserHistoriesByAadharNo(aadharno: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromaadharno/' + aadharno);
  }

  getUserHistoriesByEmail(email: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromemail/' + email);
  }

  getUserHistoriesByCandidateId(candidateid: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromcandidateid/' + candidateid);
  }
  
  getUserHistoriesByApplicationNo(applicationno: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromapplicationno/' + applicationno);
  }
  
  getUserHistoryFromProspectiveId(id: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromapplicationno/' + id);
  }
  
  getContactResults() {
    return this.http.get<IContactResult[]>(this.apiUrl + 'UserHistory/contactresults')
  }

  updateCandidateHistory(model: any) {
    return this.http.put<IUserHistoryReturnDto>(this.apiUrl + 'UserHistory', model);
  }

  updateCandidateHistoryItems(items: IUserHistoryItem[]) {
    return this.http.put(this.apiUrl + 'UserHistory/items', items);
  }

  setUserParams(params: userHistoryParams) {
      this.histParams = params;
    }
    
  getUserParams() {
      return this.histParams;
    }

  setHeaderParams(h: userHistoryHeaderParams) {
    this.headerParams = h;
  }
  getHeaderParams() {
    return this.headerParams;
  }
    composeEmailMessageOfConsent() {

    

  }

}
