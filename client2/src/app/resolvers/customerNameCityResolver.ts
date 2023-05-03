import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ClientsService } from "../clients/clients.service";
import { ICustomerNameAndCity } from "../shared/models/admin/customernameandcity";

@Injectable({
     providedIn: 'root'
 })
 export class CustomerNameCityResolver implements Resolve<ICustomerNameAndCity[]> {
 
     constructor(private service: ClientsService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICustomerNameAndCity[]> {
        return this.service.getCustomerAndCities();
     }
 
 }