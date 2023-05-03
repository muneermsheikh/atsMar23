import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { ClientsService } from '../clients/clients.service';
import { IClientIdAndNameDto } from '../shared/dtos/admin/clientIdAndNameDto';

@Injectable({
  providedIn: 'root'
})
export class AgentsResolver implements Resolve<IClientIdAndNameDto[]> {
  
  constructor(private customerService: ClientsService) {}
 
  resolve(): Observable<IClientIdAndNameDto[]> {
    return this.customerService.getAgentIdAndNames();
  }
}
