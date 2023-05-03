import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { EmployeeService } from "../employee/employee.service";
import { IEmployee } from "../shared/models/employee";

@Injectable({
     providedIn: 'root'
 })
 export class EmployeeFromIdResolver implements Resolve<IEmployee> {
 
     constructor(private empService: EmployeeService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IEmployee> {
        var id = route.paramMap.get('id');
        if (id==='') return null;

        return  this.empService.getEmployee(+id);
        
     }
 
 }