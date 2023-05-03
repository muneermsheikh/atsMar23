import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MastersService } from "../masters/masters.service";
import { IPaginationCategory } from "../shared/pagination/paginationCategory";

@Injectable({
     providedIn: 'root'
 })
 export class CategoriesResolver implements Resolve<IPaginationCategory> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(): Observable<IPaginationCategory> {
        return this.mastersService.getCategoryPaginated();
     }
 
 }