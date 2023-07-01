import { Injectable } from '@angular/core';
import { ICustomerCity } from '../shared/models/admin/customerCity';
import { ICustomer } from '../shared/models/admin/customer';
import { IClientIdAndNameDto } from '../shared/dtos/admin/clientIdAndNameDto';
import { environment } from 'src/environments/environment';
import { paramsCustomerCache } from '../shared/params/admin/paramsCustomerCache';
import { HttpClient, HttpParams } from '@angular/common/http';
import { paramsCustomer } from '../shared/params/admin/paramsCustomer';
import { of } from 'rxjs';
import { IPagination } from '../shared/models/pagination';
import { map } from 'rxjs/operators';
import { ICustomerOfficialDto } from '../shared/models/admin/customerOfficialDto';
import { ICustomerNameAndCity } from '../shared/models/admin/customernameandcity';

@Injectable({
  providedIn: 'root'
})
export class ClientsService {

  baseUrl = environment.apiUrl;
  customerType: string = 'customer';
  customerCacheDto: IClientIdAndNameDto[]=[];
  cParams= new paramsCustomerCache();

  cache = new Map();

  constructor(private http: HttpClient) { }

  getClientIdAndNames() {

    if(this.customerCacheDto !== undefined && this.customerCacheDto.length > 0) return of(this.customerCacheDto);
    
    return this.http.get<IClientIdAndNameDto[]>(this.baseUrl + 'customers/clientidandnames');
  }

  setParams(prm: paramsCustomerCache) {
    this.cParams = prm;
  }

  getparams(): paramsCustomerCache {
    return this.cParams;
  }
  
  getCustomers(custParams: paramsCustomer) {
    let params = new HttpParams();
    if (custParams.customerCityName !== "") {
      params = params.append('customerCityName', custParams.customerCityName!);
    }
    if (custParams.customerIndustryId !== 0) {
      params = params.append('customerIndustryId', custParams.customerIndustryId!.toString());
    }

    if (custParams.search) {
      params = params.append('search', custParams.search);
    }

    this.customerType = custParams.customerType ?? "customer";
    params = params.append('customerType', this.customerType);

    
    params = params.append('sort', custParams.sort);
    
    params = params.append('pageIndex', custParams.pageNumber.toString());
    params = params.append('pageSize', custParams.pageSize.toString());
    
    return this.http.get<IPagination<ICustomer>>(this.baseUrl + 'customers', {observe: 'response', params})
      .pipe(
        map(response => {
          return response.body;
        })
      )
  }

  getCustomer(id: number){
    return this.http.get<ICustomer>(this.baseUrl + 'customers/byid/' + id);
  }

  getCustomerCities() {
    return this.http.get<ICustomerCity[]>(this.baseUrl + 'customers/customerCities/' + this.customerType);
  }

  createCustomer(model: ICustomer) {
    return this.http.post(this.baseUrl + 'customers', model);
  }

  updateCustomer(model: any) {
    return this.http.put(this.baseUrl + 'customers', model);
  }

  //associates
  getAgents() {
    return this.http.get<ICustomerOfficialDto[]>(this.baseUrl + 'customers/agentdetails');
  }

  getAgentIdAndNames() {
    return this.http.get<IClientIdAndNameDto[]>(this.baseUrl + 'customers/associateidandnames/associate');
  }

  //**todo - implement procedure in contoller**/
  getCustomerAndCities() {
    return this.http.get<ICustomerNameAndCity[]>(this.baseUrl + 'customers/associateidandnames/associate');
  }
}
