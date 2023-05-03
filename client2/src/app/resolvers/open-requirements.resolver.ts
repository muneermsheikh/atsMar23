import { Injectable } from '@angular/core';
import {Resolve} from '@angular/router';
import { Observable } from 'rxjs';
import { OrderService } from '../orders/order.service';
import { IOrderItemBriefDto } from '../shared/dtos/admin/orderItemBriefDto';


@Injectable({
  providedIn: 'root'
})
export class OpenRequirementsResolver implements Resolve<IOrderItemBriefDto[]> {

  constructor(private orderService: OrderService){}

  resolve(): Observable<IOrderItemBriefDto[]> {
    
      return this.orderService.getOrderItemsBriefDto();
  }
}
