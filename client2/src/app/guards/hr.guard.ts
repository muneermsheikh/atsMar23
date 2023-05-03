import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class HrGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }
  
  /*
  canActivate(): Observable<boolean|false> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (
          user?.roles?.includes('Admin') || user?.roles?.includes('HRManager') || user?.roles?.includes('HRSupervisor') 
          || user?.roles?.includes('HRExecutive') || user?.roles?.includes('HRTrainee')
        ) {
          return true;
        } else {
          this.toastr.error('Unauthorized - This role requires HR privileges');
          return false;
        }
        
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
