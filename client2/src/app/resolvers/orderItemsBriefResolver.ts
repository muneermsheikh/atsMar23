import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderitemsService } from "../orders/orderitems.service";
import { IOrderItemBriefDto } from "../shared/models/orderItemBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class OrderItemsBriefResolver implements Resolve<IOrderItemBriefDto[]> {
 
     constructor(private service: OrderitemsService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IOrderItemBriefDto[]> {
        return this.service.getOrderItems(+route.paramMap.get('id'), true);
     }
 
 }