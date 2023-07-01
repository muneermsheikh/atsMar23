import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IPagination } from "../shared/models/pagination";
import { IEmployment } from "../shared/models/admin/employment";
import { EmploymentService } from "../employments/employment.service";
import { employmentParams } from "../shared/params/admin/employmentParam";


@Injectable({
     providedIn: 'root'
 })
 export class EmploymentsResolver implements Resolve<IPagination<IEmployment[]> | undefined> {
 
     constructor(private empService: EmploymentService) {}
    
     resolve(): Observable<IPagination<IEmployment[]> | undefined> {
        console.log('entered emplomentsresolver');
        this.empService.setEParams(new employmentParams());

        return  this.empService.getEmployments(false);
     }
 
 }