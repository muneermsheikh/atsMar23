import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { AdminService } from "../account/admin.service";
import { IUser } from "../shared/models/user";
import { IPaginationUser } from "../shared/pagination/paginationUser";
import { UserParams } from "../shared/params/userParams";

@Injectable({
     providedIn: 'root'
 })
 export class UsersWithRolesResolver implements Resolve<IPaginationUser> {
 
     constructor(private adminService: AdminService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPaginationUser> {
        var userParams = new UserParams();
        var userType = route.paramMap.get('userType');
        userParams.userType = userType;
        this.adminService.setParams(userParams);

        return this.adminService.getUsersWithRolesPaginated(false);
     }
 
 }