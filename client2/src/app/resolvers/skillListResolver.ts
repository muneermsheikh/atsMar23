import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ISkillData } from "../shared/models/skillData";
import { SharedService } from "../shared/services/shared.service";


@Injectable({
     providedIn: 'root'
 })
 export class SkillListResolver implements Resolve<ISkillData[]> {
 
     constructor(private sharedService: SharedService) {}
 
     resolve(): Observable<ISkillData[]> {
        var sdata = this.sharedService.getSkillData();
        return sdata;
     }

 }