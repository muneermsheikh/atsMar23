import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MastersService } from "../masters/masters.service";
import { IQualification } from "../shared/models/hr/qualification";

@Injectable({
     providedIn: 'root'
 })
 export class QualificationListResolver implements Resolve<IQualification[]> {
 
     constructor(private qService: MastersService) {}
 
     resolve(): Observable<IQualification[]> {
        console.log('entered qualifictionListResolver');
        var qs = this.qService.getQualificationList();
        console.log('qs:', qs);
        return qs;
     }

 }