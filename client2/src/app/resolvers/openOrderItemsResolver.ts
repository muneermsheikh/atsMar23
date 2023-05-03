import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IOrderItemBriefDto } from "../shared/models/orderItemBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class OpenOrderItemsResolver implements Resolve<IOrderItemBriefDto[]> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(): Observable<IOrderItemBriefDto[]> {
        return this.orderService.getOrderItemsBriefDto();
     }
 
 }