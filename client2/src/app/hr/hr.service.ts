import { Injectable } from '@angular/core';
import { IProfession } from '../shared/models/masters/profession';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ReplaySubject, of } from 'rxjs';
import { IUser } from '../shared/models/admin/user';
import { assessmentQBankParams } from '../shared/models/admin/assessmentQBankParams';
import { IPagination } from '../shared/models/pagination';
import { IAssessmentQBank } from '../shared/models/admin/assessmentQBank';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account/account.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IChecklistHRDto } from '../shared/dtos/hr/checklistHRDto';

@Injectable({
  providedIn: 'root'
})
export class HrService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  

  qParams = new assessmentQBankParams();
  pagination?: IPagination<IAssessmentQBank[]>|null;  // = new paginationQBank();
  existingCats: IProfession[]=[];
  routeId: string;
  user?: IUser;
  cache = new Map();

  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router,
    private accountService:AccountService,
    private http: HttpClient) {
      this.routeId = this.activatedRoute.snapshot.params['id'];
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    }
  
   
    getExistingProfFromQBank() {
      return this.http.get<IProfession[]>(this.apiUrl + 'assessmentqbank/existingqbankcategories');
    }

    getAssessmentQBank() {
      return this.http.get<IAssessmentQBank[]>(this.apiUrl + 'assessmentqbank/assessmentbankqs');
    }

    getQBank(useCache: boolean) {
      if (useCache === false) this.cache = new Map();

      if(this.cache.size > 0 && useCache === true) {
        if(this.cache.has(Object.values(this.qParams).join('-'))) {
          this.pagination!.data=this.cache.get(Object.values(this.qParams).join('-'));
          return of(this.pagination);
        }
      }

      let params = new HttpParams();
      if(this.qParams.categoryId !== 0) {
        params = params.append('categoryId', this.qParams.categoryId!.toString());
      }
      if(this.qParams.categoryName !== '') {
        params = params.append('categoryName', this.qParams.categoryName!);
      }
      if (this.qParams.search) {
        params = params.append('search', this.qParams.search);
      }
  
      params = params.append('sort', this.qParams.sort);
      params = params.append('pageIndex', this.qParams.pageNumber.toString());
      params = params.append('pageSize', this.qParams.pageSize.toString());

      return this.http.get<IPagination<IAssessmentQBank[]>>(this.apiUrl + 'assessmentqbank/assessmentbankqs', {observe: 'response', params})
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.qParams).join('-'), response.body!.data);
            this.pagination = response.body;
            return response.body;
          })
        )
    }

    getQs(categoryName: string) {
      return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentQBank/categoryName/' + categoryName);
    }

    getQBankByCategoryId(id: number) {
      return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentQBank/qbank/' + id);
    }

    
    setQParams(params: assessmentQBankParams) {
      this.qParams = params;
    }
    
    getQParams() {
      return this.qParams;
    }

    getQBankCategories() {
      if (this.existingCats.length > 0) return of(this.existingCats);

      return this.http.get<IProfession[]>(this.apiUrl + 'assessmentQBank/existingqbankcategories')
        .pipe(
          map(response => {
            this.existingCats = response;
            return response;
          })
        )
    }

    insertQBank(model: any) {
      return this.http.post(this.apiUrl + 'assessmentQBank', model);
    }

    updateQBank(model: any) {
      return this.http.put(this.apiUrl + 'assessmentQBank', model);
    }

    
    //checklist
    getChecklistHRDto(candidateid: number, orderitemid: number) {
      return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/checklisthr/' + candidateid + '/' + orderitemid);
    }
}
