import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IPaginationOrderBrief } from "../shared/pagination/pagnationBriefDto";

@Injectable({
     providedIn: 'root'
 })
 export class OrdersBriefResolver implements Resolve<IPaginationOrderBrief> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(): Observable<IPaginationOrderBrief> {
        
        return this.orderService.getOrdersBrief(false);
     }
 
 }