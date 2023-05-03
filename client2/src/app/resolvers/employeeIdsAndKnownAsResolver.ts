import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IEmployeeIdAndKnownAs } from "../shared/models/admin/employeeIdAndKnownAs";
import { SharedService } from "../shared/shared.service";
import { MastersService } from "../masters/masters.service";

@Injectable({
     providedIn: 'root'
 })
 export class EmployeeIdsAndKnownAsResolver implements Resolve<IEmployeeIdAndKnownAs[]> {
 
     constructor(private mastersService: MastersService) {}
 
     resolve(): Observable<IEmployeeIdAndKnownAs[]> {
        return this.mastersService.getEmployeeIdAndKnownAs();
     }
 
 }