import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { IDLForwardToAgent } from "../shared/models/admin/dlforwardToAgent";
import { DlForwardService } from "../orders/dl-forward.service";



@Injectable({
     providedIn: 'root'
 })
 export class DLForwardsOfAnOrderIdResolver implements Resolve<IDLForwardToAgent[]|null> {
 
     constructor(private dlfwdService: DlForwardService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IDLForwardToAgent[]|null> {

        var routeid = route.paramMap.get('orderid');
        //console.log('candidateResolver routei is:',routeid);
        if (routeid === '0' || routeid=== null  ) return of(null);
        return this.dlfwdService.getDLForwardsOfAnOrderId(+routeid);
     }
 
 }