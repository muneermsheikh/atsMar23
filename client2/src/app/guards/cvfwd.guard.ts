import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class CvfwdGuard implements CanActivate {
  
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }
  /*
  canActivate(): Observable<boolean | undefined > {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (
          user?.roles?.includes('Admin') || user?.roles?.includes('DocumentControllerAdmin')
        ) {
          return true;
        }
        this.toastr.error('Unauthorizzed - this role requires Document Controller - Admin privileges or Admin');
      })
    )
  }
  */

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(auth => {
        if (auth) return true;
        else {
          this.router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false
        }
      })
    );
  }
}
