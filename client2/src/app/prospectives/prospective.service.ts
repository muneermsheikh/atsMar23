import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { ReplaySubject, of } from 'rxjs';
import { prospectiveCandidateParams } from '../shared/params/hr/prospectiveCandidateParams';
import { prospectiveSummaryParams } from '../shared/params/hr/prospectiveSummaryParams';
import { IProspectiveCandidate } from '../shared/models/hr/prospectiveCandidate';
import { IPagination } from '../shared/models/pagination';
import { IProspectiveSummaryDto } from '../shared/dtos/hr/propectiveSummaryDto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { IProspectiveRegisterToAddDto } from '../shared/dtos/hr/prospectiveRegisterToAddDto';
import { IProspectiveUpdateDto } from '../shared/dtos/hr/prospectiveUpdateDto';

@Injectable({
  providedIn: 'root'
})
export class ProspectiveService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new prospectiveCandidateParams();
  sParams = new prospectiveSummaryParams();
  //paginationSummary = new PaginationProspectiveSummary();
  summaries: IProspectiveSummaryDto[]=[];
  pagination?: IPagination<IProspectiveCandidate[]>; // = new PaginationProspectiveCandidates();
  cache = new Map();
  cacheSummary = new Map();

  constructor(private http: HttpClient) { }

  getProspectiveCandidates(useCache: boolean) {
        if (useCache === false) this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.oParams).join('-'))) {
        this.pagination!.data = this.cache.get(Object.values(this.oParams).join('-'));
        return of(this.pagination);
      }
    }

    let params = new HttpParams();
    
    if (this.oParams.status !== ''  && this.oParams.status !== undefined)  params = params.append('status', this.oParams.status);
    if (this.oParams.categoryRef !== ''  && this.oParams.categoryRef !== undefined)  params = params.append('categoryRef', this.oParams.categoryRef);

    if (this.oParams.dateAdded !=='' && this.oParams.dateAdded !== undefined ){
      params = params.append('dateAdded', this.oParams.dateAdded);
    }
    //if(this.oParams.status !== '') params = params.append('status', this.oParams.status);
    
    if (this.oParams.search) params = params.append('search', this.oParams.search);
    
    params = params.append('sort', this.oParams.sort);
    params = params.append('pageIndex', this.oParams.pageNumber.toString());
    params = params.append('pageSize', this.oParams.pageSize.toString());

    return this.http.get<IPagination<IProspectiveCandidate[]>>(this.apiUrl + 'prospectivecandidates', {params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(this.oParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
    }
  
  getProspectiveSummary(useCache: boolean)
  {
    if (useCache === false) this.cacheSummary = new Map();
    
    if (this.cacheSummary.size > 0 && useCache === true) {
      if (this.cacheSummary.has(Object.values(this.sParams).join('-'))) {
        this.summaries= this.cacheSummary.get(Object.values(this.sParams).join('-'));
        return of(this.summaries);
      }
    }

    let params = new HttpParams();
    
    if (this.sParams.status !== ''  && this.sParams.status !== undefined)  params = params.append('status', this.sParams.status);
    if (this.sParams.categoryRef !== ''  && this.sParams.categoryRef !== undefined)  params = params.append('categoryRef', this.sParams.categoryRef);
    if (this.sParams.dated !=='' && this.sParams.dated !== undefined ){
      //console.log('dateAdded', this.sParams.dated);
      params = params.append('dateAdded', this.sParams.dated);
    }
    
    if (this.oParams.search) params = params.append('search', this.sParams.search);
    
    params = params.append('sort', this.sParams.sort);
    //params = params.append('pageIndex', this.sParams.pageNumber.toString());
    //params = params.append('pageSize', this.sParams.pageSize.toString());

    return this.http.get<IProspectiveSummaryDto[]>(this.apiUrl + 'ProspectiveCandidates/summary', {params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(this.sParams).join('-'), response);
          this.summaries = response;
          return response;
        })
      )
  }
  
  setParams(params: prospectiveCandidateParams) {
    this.oParams = params;
    console.log('in prospective.sevice, params set to', this.oParams);
  }
  
  getParams() {
    return this.oParams;
  }

  setSummaryParams(params: prospectiveSummaryParams) {
    this.sParams = params;
    console.log('summaryParams set to:', params);
  }

  getSummaryParams() {
    return this.sParams;
  }

  displayModalUserContact() {
    
  }

  uploadProspectiveXLSFile(prospectivexlFile: any){
    console.log('prosective upload in prospecive service');
    return this.http.post<string>(this.apiUrl + 'excel/uploadProspectiveFile', prospectivexlFile)
  }

  convertProspectiveXLStoDb(filename: string) {
    return this.http.post<string>(this.apiUrl + 'excel/convertToDb/' + filename, {});
  }
  createCandidateFromprospective(model: IProspectiveRegisterToAddDto) {
    return this.http.post(this.apiUrl + 'prospectivecandidates', model )
  }

  updateProspectives(model: IProspectiveUpdateDto[]) {
    return this.http.put(this.apiUrl + 'prospectivecandidates/prospectivelistedit', model);
  }
}
