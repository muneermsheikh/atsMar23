import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { OrderService } from "../orders/order.service";
import { IOpenOrderItemCategoriesDto } from "../shared/dtos/admin/openOrderItemCategriesDto";


@Injectable({
     providedIn: 'root'
 })
 export class OpenOrderItemCategoriesResolver implements Resolve<IOpenOrderItemCategoriesDto[]|undefined> {
 
     constructor(private orderService: OrderService) {}
 
     resolve(): Observable<IOpenOrderItemCategoriesDto[]|undefined> {
        return this.orderService.getOpenOrderItemCategoriesDto();
     }
 
 }