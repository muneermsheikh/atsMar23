import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ITaskType } from "../shared/models/admin/taskType";
import { UserTaskService } from "../userTask/user-task.service";


@Injectable({
     providedIn: 'root'
 })
 export class TaskTypeResolver implements Resolve<ITaskType[]> {
 
     constructor(private service: UserTaskService) {}
 
     resolve(): Observable<ITaskType[]> {
     
        return this.service.getTaskTypes();
     }
 
 }