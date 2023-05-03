import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { IDLForwardCategory } from "../shared/models/admin/dlForwardCategory";
import { DlForwardService } from "../orders/dl-forward.service";



@Injectable({
     providedIn: 'root'
 })
 export class AssociateForwardsForADLResolver implements Resolve<IDLForwardCategory[] | null> {
 
     constructor(private dlfwdService: DlForwardService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IDLForwardCategory[] | null> {

        var routeid = route.paramMap.get('orderid');
        if (routeid === null || routeid === '') return of(null);

        return this.dlfwdService.getAssociatesForwardedForADL(+routeid);
     }
 
 }