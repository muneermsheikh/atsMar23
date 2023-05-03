import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IOrderBriefDto } from "../shared/models/orderBriefDto";
import { IOrderItemBriefDto } from "../shared/models/orderItemBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class OrderBriefResolver implements Resolve<IOrderBriefDto> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IOrderBriefDto> {
        var brf = this.orderService.getOrderBrief(+route.paramMap.get('id'));
        console.log(brf.subscribe(response => {
            console.log('in resolver',response);
        }))
        return brf;
     }
 
 }