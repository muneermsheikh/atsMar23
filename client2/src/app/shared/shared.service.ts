import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ICustomerNameAndCity } from './models/admin/customernameandcity';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { of } from 'rxjs';
import { ISkillData } from './models/hr/skillData';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  apiUrl = environment.apiUrl;
  agents: ICustomerNameAndCity[]=[];
  customers: ICustomerNameAndCity[]=[];

  constructor(private http: HttpClient) { }
  
  getAgents() {
    if (this.agents.length > 0) {
      return of(this.agents);
    }
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/associateidandnames/associate').pipe(
      map((response: any) => {
        this.agents = response;
        return response;
      })
    );
  }

  getCustomers() {
    if (this.customers.length > 0) {
      return of(this.customers);
    }
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/associateidandnames/customer').pipe(
      map(response => {
        this.customers = response;
        return response;
      })
    );
  }

  
  getSkillData() {
    return this.http.get<ISkillData[]>(this.apiUrl + 'masters/skillDatalist');
  }

 
  checkAadharNoExists(aadharNo: string) {
    return this.http.get<boolean>(this.apiUrl + 'account/' + aadharNo);
  }


  getCustomerOfficialIds() {
    
  }

  /*
  getDropDownText(id: number, object: any){
    const selObj = _.filter(object, function (o: any) {
        return ( _.includes(id,o.id));
    });
    return selObj;
  }
*/

}
