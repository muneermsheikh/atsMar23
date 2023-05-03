import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IOrder } from "../shared/models/admin/order";


@Injectable({
     providedIn: 'root'
 })
 export class OrderResolver implements Resolve<IOrder|null> {
 
     constructor(private orderService: OrderService) {}
        
     resolve(route: ActivatedRouteSnapshot): Observable<IOrder|null> {
        var id=route.paramMap.get('id');
        if(id===null) return of(null);
        return this.orderService.getOrder(+id);
     }
 
 }