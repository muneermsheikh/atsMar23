import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";

import { IOrderBriefDto, OrderBriefDto } from "../shared/dtos/admin/orderBriefDto";
import { IPagination } from "../shared/models/pagination";

@Injectable({
     providedIn: 'root'
 })
 export class OrdersBriefResolver implements Resolve<IPagination<IOrderBriefDto[]>> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(): Observable<IPagination<IOrderBriefDto[]>> {
        
        return this.orderService.getOrdersBrief(false);
     }
 
 }