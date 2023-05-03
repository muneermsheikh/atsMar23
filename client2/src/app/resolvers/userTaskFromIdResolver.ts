import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { UserTaskService } from "../userTask/user-task.service";
import { IApplicationTask } from "../shared/models/admin/applicationTask";

@Injectable({
     providedIn: 'root'
 })
 export class UserTaskFromIdResolver implements Resolve<IApplicationTask|undefined> {
 
     constructor(private taskService: UserTaskService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IApplicationTask|undefined> {

        var id = route.paramMap.get('id');   
        if(id===null) return of(undefined);
        return this.taskService.getTask(+id);  
         
     }
 
 }