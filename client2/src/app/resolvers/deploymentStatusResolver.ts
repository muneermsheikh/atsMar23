import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { DeployService } from "../deploys/deploy.service";
import { IDeploymentStatus } from "../shared/models/masters/deployStatus";
import { IDeployStage } from "../shared/models/masters/deployStage";

@Injectable({
     providedIn: 'root'
 })
 export class DeploymentStatusResolver implements Resolve<IDeployStage[]> {
 
     constructor(private processService: DeployService) {}
 
     resolve(): Observable<IDeployStage[]> {
        return this.processService.getDeployStatus();
     }
 
 }