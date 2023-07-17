import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { transactionParams } from '../shared/params/finance/tranactionParams';
import { FinanceVoucher, IFinanceVoucher } from '../shared/models/masters/finance/financeVoucher';
import { IPagination } from '../shared/models/pagination';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { IApiReturnDto } from '../shared/dtos/admin/apiReturnDto';
import { IVoucherToAddNewPaymentDto } from '../shared/dtos/finance/voucherToAddNewPaymentDto';
import { IStatementofAccountDto } from '../shared/dtos/finance/statementOfAccountDto';

@Injectable({
  providedIn: 'root'
})
export class VouchersService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new transactionParams();
  transactions: IFinanceVoucher[]=[];
  pagination?: IPagination<IFinanceVoucher[]>;
  count = 0;
  cache = new Map();
  
  constructor(private http: HttpClient) { }
  
  getVouchers(useCache: boolean) {
    if (useCache === false) this.cache = new Map();

    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.sParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.sParams).join('-'));
        return of(this.pagination);
      }
    }

    let params = new HttpParams();

    if (this.sParams.coaId !== 0 )  params = params.append('coaId', this.sParams.coaId.toString());
    
    if (this.sParams.search) params = params.append('search', this.sParams.search);

    params = params.append('sort', this.sParams.sort);
    params = params.append('pageIndex', this.sParams.pageNumber.toString());
    params = params.append('pageSize', this.sParams.pageSize.toString());

    return this.http.get<IPagination<IFinanceVoucher[]>>(this.apiUrl + 'finance/vouchers', {params})
      .pipe(
        map((response: any) => {
          this.cache.set(Object.values(this.sParams).join('-'), response);
          this.pagination = response;
          this.count = response.count;
          return response;
        })
      )
    }

  getVoucherFromId( id: number): any {
    var voucher: IFinanceVoucher;
    if(id===0) return of(undefined);
    
    return this.http.get<IFinanceVoucher|undefined>(this.apiUrl + 'finance/vouchers/' + id);
  }

  deleteVoucher(id: number)
  {
    return this.http.delete<boolean>(this.apiUrl + 'finance/voucher/' + id);
  }

  insertVoucherWithUploads(model: FormData) {
    return this.http.post<IApiReturnDto>(this.apiUrl+ 'finance/RegisterNewVoucher', model, {
      reportProgress: true,
      observe: 'events'
    });
  }

  insertVoucher(model: IVoucherToAddNewPaymentDto) {
    console.log('voucher service insertvoucher model:', model)
    return this.http.post<IApiReturnDto>(this.apiUrl+ 'finance/newpaymentfromcandidate', model, {
      reportProgress: true,
      observe: 'events'
    });
  }

  public updateWithFiles(formData: FormData) {
    return this.http.put(this.apiUrl + 'finance/updateVoucherWithFiles', formData, {
      reportProgress: true,
      observe: 'events',
    });
  }

  getNextVoucherNo() {
      return this.http.get<number>(this.apiUrl + 'finance/nextvoucherno');
  }
  
  setParams(params: transactionParams) {
    this.sParams = params;
  }
  
  getParams() {
    return this.sParams;
  }

  getStatementOfAccount(accountid: number, fromdate: string, uptodate: string) {

    return this.http.get<IStatementofAccountDto>(this.apiUrl + 'finance/statementofaccount/' + accountid + '/' + fromdate + '/' + uptodate);
  
  }  

  deleteVoucherFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.count--;
  }
}
