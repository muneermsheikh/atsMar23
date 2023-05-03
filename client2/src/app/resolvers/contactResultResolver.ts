import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IContactResult } from "../shared/models/contactResult";
import { UserTaskService } from "../userTask/user-task.service";


@Injectable({
     providedIn: 'root'
 })
 export class TaskTypeResolver implements Resolve<IContactResult[]> {
 
     constructor(private service: UserTaskService) {}
 
     resolve(): Observable<IContactResult[]> {
     
        return this.service.getContactResults();
     }
 
 }