import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ICustomerOfficialDto } from "../shared/models/admin/customerOfficialDto";
import { MastersService } from "../masters/masters.service";
import { ClientsService } from "../clients/clients.service";



@Injectable({
     providedIn: 'root'
 })
 export class CustomerOfficialsResolver implements Resolve<ICustomerOfficialDto[]> {
 
     constructor(private service: ClientsService) {}
 
     resolve(): Observable<ICustomerOfficialDto[]> {
        return this.service.getAgents();
     }
 
 }