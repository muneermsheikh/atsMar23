import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class SelectionsGuard implements CanActivate {
  
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }
  
  /*
  canActivate(): Observable<boolean|undefined> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (
          user?.roles?.includes('Admin') || user?.roles?.includes('HRManager') 
          || user?.roles?.includes('DocumentControllerAdmin') || user?.roles?.includes('Selection') 
        ) {
          return true;
        }
        this.toastr.error('Unauthorized - This role requires Admin or HR Manager or Selection privileges');
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
