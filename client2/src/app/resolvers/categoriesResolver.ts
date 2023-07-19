import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MastersService } from "../masters/masters.service";
import { IPagination } from "../shared/models/pagination";
import { IProfession } from "../shared/models/masters/profession";

@Injectable({
     providedIn: 'root'
 })
 export class CategoriesResolver implements Resolve<IPagination<IProfession[]> | undefined> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(): Observable<IPagination<IProfession[]> | undefined> {
        return this.mastersService.getCategories(false);
     }
 
 }