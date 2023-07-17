import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { IPagination } from "../shared/models/pagination";
import { ICVReferredDto } from "../shared/dtos/admin/cvReferredDto";
import { CvrefService } from "../admin/cvref.service";
import { CVRefParams } from "../shared/params/admin/cvRefParams";

@Injectable({
     providedIn: 'root'
 })
 export class CVsReferredResolver implements Resolve<IPagination<ICVReferredDto[]> | undefined> {
 
     constructor(private refservice: CvrefService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPagination<ICVReferredDto[]> | undefined>{
        var refparams = new CVRefParams();
        var id = route.paramMap.get('orderid');
        if(id==='' || id==='0' || id === null) {
           return of(undefined);
        } else {
            refparams.orderId=+id;
        }

        this.refservice.setCVRefParams(refparams);
        return this.refservice.referredCVs(false);
     }
 
 }