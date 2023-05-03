import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizedGuard implements CanActivate {

  constructor(private accountService: AccountService, private toastr: ToastrService) { }
  
  canActivate(): Observable<boolean|false> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user) {
          return true;
        } else {
          this.toastr.error('Unauthorizzed');
          return false;
        }
        
      })
    )
  }
}
