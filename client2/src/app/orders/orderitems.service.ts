import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { orderItemParams } from '../shared/params/admin/orderItemParams';
import { IOrderItemBriefDto } from '../shared/dtos/admin/orderItemBriefDto';

@Injectable({
  providedIn: 'root'
})
export class OrderitemsService {
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new orderItemParams();
  cache = new Map();
  orderitems: IOrderItemBriefDto[]=[];


  constructor(private http: HttpClient) { }

    getOrderItems(orderid: number, useCache: boolean) {
      if(useCache === false) this.cache = new Map();
      if(this.cache.size > 0 && useCache === true) {
          if (this.cache.has(Object.values(this.oParams).join('-'))) {
            this.orderitems = this.cache.get(Object.values(this.oParams).join('-'));
            return of(this.orderitems);
        }
      }

      return this.http.get<IOrderItemBriefDto[]>(this.apiUrl + 'orders/orderitemsbyorderid/'+orderid)
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.oParams).join('-'), response);
            this.orderitems = response;
            return response;
          })
        )
    }
  
    getOrderItem(orderitem: number) {
    let item: IOrderItemBriefDto|undefined;
    this.cache.forEach((items: IOrderItemBriefDto[]) => {
      item = items.find(p => p.orderItemId === orderitem);
    })
    
    if (item) return of(item);
    
    var oitem = this.http.get<IOrderItemBriefDto>(this.apiUrl + 'orders/itemdtobyid/' + orderitem);
    return oitem;
  }

  getOrderItemRefCode(orderitemid: number) {
    /*let item: IOrderItemBriefDto|undefined;

    this.cache.forEach((items: IOrderItemBriefDto[]) => {
      item = items.find(p => p.orderItemId === orderitemid);
    })
    
    if (item) return of(item);
    
    return this.http.get<string>(this.apiUrl + 'orders/refcodefromorderitemid/' + orderitemid)
    .pipe(
      map(response => {
        this.cache.set(Object.values(this.oParams).join('-'), response);
        return response;
      })
    )
    */
    
    return this.http.get<string>(this.apiUrl + 'orders/refcodefromorderitemid/' + orderitemid);

  }

  

}

  