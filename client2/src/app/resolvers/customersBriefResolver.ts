import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { CustomersService } from "../customers/customers.service";
import { ICustomerBriefDto } from "../shared/dtos/admin/customerBriefDto";
import { IPagination } from "../shared/models/pagination";
import { customerParams } from "../shared/models/admin/customerParams";

@Injectable({
     providedIn: 'root'
 })
 export class CustomersBriefResolver implements Resolve<IPagination<ICustomerBriefDto[]>|null> {
 
     constructor(private customerService: CustomersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPagination<ICustomerBriefDto[]>|null> {
        var params = new customerParams();
        params.custType='customer';
        this.customerService.setCustParams(params);
        return this.customerService.getCustomers(false);
     }
 
 }