import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MastersService } from "../masters/masters.service";
import { IProfession } from "../shared/models/profession";

@Injectable({
     providedIn: 'root'
 })
 export class CategoryResolver implements Resolve<IProfession> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IProfession> {
        return this.mastersService.getCategory(+route.paramMap.get('id'));
     }
 
 }