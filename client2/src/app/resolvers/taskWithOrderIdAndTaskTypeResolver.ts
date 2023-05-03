import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IApplicationTask } from "../shared/models/applicationTask";
import { UserTaskService } from "../userTask/user-task.service";

@Injectable({
     providedIn: 'root'
 })
 export class TaskWithOrderIdAndTaskTypeResolver implements Resolve<IApplicationTask> {
 
     constructor(private taskService: UserTaskService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IApplicationTask> {
        
        var orderid = +route.paramMap.get('orderid');
        var tasktypeid = +route.paramMap.get('tasktypeid');
        console.log('in resolver',orderid, tasktypeid);
        return this.taskService.getTaskByOrderIdAndTaskType(orderid, tasktypeid);

     }
 
 }