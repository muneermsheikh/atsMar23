import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IApplicationTaskInBrief } from "../shared/models/admin/applicationTaskInBrief";
import { UserTaskService } from "../userTask/user-task.service";

@Injectable({
     providedIn: 'root'
 })
 export class UserTaskResolver implements Resolve<IApplicationTaskInBrief[]> {
 
     constructor(private taskService: UserTaskService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IApplicationTaskInBrief[]> {

         return this.taskService.getTasks(false);  //defauls pageIndex and pageSize
         
     }
 
 }