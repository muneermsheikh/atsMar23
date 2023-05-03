import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MastersService } from "../masters/masters.service";
import { IProfession } from "../shared/models/masters/profession";

@Injectable({
     providedIn: 'root'
 })
 export class CategoryListResolver implements Resolve<IProfession[]> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(): Observable<IProfession[]> {
        
        var lst = this.mastersService.getCategoryList();
        console.log('entered categoryListResoler', lst);
        return lst;
     }
 
 }