import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { ICustomer } from "../shared/models/admin/customer";
import { CustomersService } from "../customers/customers.service";

@Injectable({
     providedIn: 'root'
 })
 export class CustomerResolver implements Resolve<ICustomer|null> {
 
     constructor(private customerService: CustomersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICustomer|null> {
        var id = route.paramMap.get('id');
        if(id==='' || id==='0' || id === null) return of(null)
        return this.customerService.getCustomer(+id);
     }
 
 }