import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { AdminService } from "../account/admin.service";
import { IUser } from "../shared/models/user";

@Injectable({
     providedIn: 'root'
 })
 export class RolesResolver implements Resolve<string[]> {
 
     constructor(private adminService: AdminService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<string[]> {
        return this.adminService.getIdentityRoles();
     }
 
 }