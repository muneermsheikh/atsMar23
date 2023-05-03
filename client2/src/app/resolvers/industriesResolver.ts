import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IIndustryType } from "../shared/models/masters/profession";
import { MastersService } from "../masters/masters.service";

@Injectable({
     providedIn: 'root'
 })
 export class IndustriesResolver implements Resolve<IIndustryType[]> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(): Observable<IIndustryType[]> {
        return this.mastersService.getIndustries();
     }
 
 }