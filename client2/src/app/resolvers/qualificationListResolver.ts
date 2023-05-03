import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IQualification } from "../shared/models/masters/profession";
import { MastersService } from "../masters/masters.service";

@Injectable({
     providedIn: 'root'
 })
 export class QualificationListResolver implements Resolve<IQualification[]> {
 
     constructor(private qService: MastersService) {}
 
     resolve(): Observable<IQualification[]> {
        var qs = this.qService.getQualificationList();
        return qs;
     }

 }