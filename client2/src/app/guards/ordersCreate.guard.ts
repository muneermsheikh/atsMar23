import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class OrdersCreateGuard implements CanActivate {

  constructor(private accountService: AccountService) { }
  
  canActivate(): Observable<boolean|false> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user?.roles?.includes('OrderCreate') ) {
          return true;
        } else{
          return false;
        }
      })
    )
  }
  
}
