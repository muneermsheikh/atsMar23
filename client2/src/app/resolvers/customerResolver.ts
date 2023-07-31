import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { Customer, ICustomer } from "../shared/models/admin/customer";
import { CustomersService } from "../customers/customers.service";

@Injectable({
     providedIn: 'root'
 })
 export class CustomerResolver implements Resolve<ICustomer|null> {
 
     constructor(private customerService: CustomersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICustomer|null> {
        console.log('entered customer resolver');
        
        var id = route.paramMap.get('id');
        var sCustType: string='';
        var custType = route.paramMap.get('custType');
        if(custType!==null) {
            sCustType=custType;
        } else {
            return of(null);
        }
        console.log('customeReslver id=', id);
        if(id==='' || id==='0' || id === null) {
            var customer = new Customer();
            customer.customerType=sCustType;
            return of(customer);
        }
        return this.customerService.getCustomer(+id);
     }
 
 }