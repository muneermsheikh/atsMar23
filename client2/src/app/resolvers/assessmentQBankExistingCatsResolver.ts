import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { HrService } from "../hr/hr.service";
import { IProfession } from "../shared/models/profession";

@Injectable({
     providedIn: 'root'
 })
 export class AssessmentQBankExistingCatsResolver implements Resolve<IProfession[]> {
 
     constructor(private qBankService: HrService) {}
 
     resolve(): Observable<IProfession[]> {
        return this.qBankService.getExistingProfFromQBank();
     }
 
 }