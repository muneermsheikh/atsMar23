import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../account/account.service';

@Injectable({
  providedIn: 'root'
})
export class ProcessGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }
  
  /*
  canActivate(): Observable<boolean|undefined> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (
          user?.roles?.includes('ProcessManager') || user?.roles?.includes('ProcessExecutive') 
          || user?.roles?.includes('MedicalExecutive') || user?.roles?.includes('MedicalExecutiveGAMMCA')
          || user?.roles?.includes('VisaExecutiveDubai') || user?.roles?.includes('VisaExecutiveKSA') 
          || user?.roles?.includes('VisaExecutiveSharjah') || user?.roles?.includes('VisaExecutiveOman') 
          || user?.roles?.includes('VisaExecutiveQatar') || user?.roles?.includes('DocumentControllerProcess')
        ) {
          return true;
        }
        this.toastr.error('Unauthorized - This role requires Document Processing privileges');
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
