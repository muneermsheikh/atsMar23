import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { IOrderItemBriefDto } from "../shared/dtos/admin/orderItemBriefDto";
import { OrderitemsService } from "../orders/orderitems.service";


@Injectable({
     providedIn: 'root'
 })
 export class OrderItemBriefResolver implements Resolve<IOrderItemBriefDto|null> {
 
     constructor(private service: OrderitemsService) {}
    
     resolve(route: ActivatedRouteSnapshot): Observable<IOrderItemBriefDto|null> {
        
        var routeid=route.paramMap.get('id');
        if(routeid==null) return of(null);
        
        var iem = this.service.getOrderItem(+routeid);
        
        return iem;
     }
 
 }