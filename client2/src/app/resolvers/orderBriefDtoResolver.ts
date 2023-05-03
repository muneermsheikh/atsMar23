import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IOrderBriefDto } from "../shared/models/orderBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class OrderBriefDtoResolver implements Resolve<IOrderBriefDto> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IOrderBriefDto> {
        var orderid = route.paramMap.get('orderid');
        if(orderid===undefined || orderid==='') return;
        return this.orderService.getOrderBrief(+orderid);
     }
 
 }