import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { EmploymentService } from "../admin/employment.service";
import { IEmploymentDto } from "../shared/dtos/employmentDto";
import { IPaginationEmployment } from "../shared/pagination/paginationEmployment";
import { employmentParams } from "../shared/params/employmentParam";

@Injectable({
     providedIn: 'root'
 })
 export class EmploymentsFromOrderNoResolver implements Resolve<IPaginationEmployment> {
 
     constructor(private empService: EmploymentService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPaginationEmployment> {
        var id = route.paramMap.get('orderno');
        if (id==='') return null;

        var eParams = new employmentParams();
        eParams.orderNo=+id;

        this.empService.setEmploymenetParams(eParams);

        return  this.empService.getEmploymentsPaginatedFromParams(false);
        
     }
 
 }