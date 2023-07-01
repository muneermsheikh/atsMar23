import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { CustomersService } from "../customers/customers.service";
import { ICustomerBriefDto } from "../shared/dtos/admin/customerBriefDto";
import { IPagination } from "../shared/models/pagination";
import { paramsCustomer } from "../shared/params/admin/paramsCustomer";

@Injectable({
     providedIn: 'root'
 })
 export class CustomersBriefResolver implements Resolve<IPagination<ICustomerBriefDto[]>|null> {
 
     constructor(private customerService: CustomersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPagination<ICustomerBriefDto[]>|null> {
        var id = route.paramMap.get('custType');
        if(id===null) return of(null);
        if(!(id==='customer' || id==='associate' || id==='vendor')) return of(null);
        
        var params = new paramsCustomer();
        params.customerType=id;
        this.customerService.setCustParams(params);
        return this.customerService.getCustomers(false);
     }
 
 }