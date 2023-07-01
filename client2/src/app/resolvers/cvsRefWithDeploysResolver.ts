import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { ICVReferredDto } from "../shared/dtos/admin/cvReferredDto";
import { CvrefService } from "../admin/cvref.service";

@Injectable({
     providedIn: 'root'
 })
 export class CVsRefWithDeploysResolver implements Resolve<ICVReferredDto | undefined> {
 
     constructor(private refservice: CvrefService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICVReferredDto | undefined>{
        var id = route.paramMap.get('cvrefid');
        if(id==='' || id==='0' || id === null) {
           return of(undefined);
        }

        return this.refservice.getCVRefWithDeploys(+id);
     }
 
 }