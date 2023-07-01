import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IPagination } from "../shared/models/pagination";
import { IQualification } from "../shared/models/hr/qualification";
import { MastersService } from "../masters/masters.service";



@Injectable({
     providedIn: 'root'
 })
 export class QualificationsResolver implements Resolve<IPagination<IQualification[]> | undefined | null> {
 
     constructor(private qService: MastersService) {}
 
     /*resolve(): Observable<IQualification[]> {

        var parm = new qualificationParams();
        parm.id = 
        return this.qService.getQualifications(false);
     }
     */
 
     resolve(): Observable<IPagination<IQualification[]> | undefined | null > {
        var qs = this.qService.getQualifications(false);
        return qs;
     }

 }