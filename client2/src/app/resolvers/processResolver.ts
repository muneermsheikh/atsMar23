import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ProcessingService } from "../process/processing.service";
import { IPaginationDeploy } from "../shared/pagination/paginationDeploy";

@Injectable({
     providedIn: 'root'
 })
 export class ProcessResolver implements Resolve<IPaginationDeploy> {
 
     constructor(private processService: ProcessingService) {}
 
     resolve(): Observable<IPaginationDeploy> {
        return this.processService.getProcessesPaginated(false);
     }
 
 }