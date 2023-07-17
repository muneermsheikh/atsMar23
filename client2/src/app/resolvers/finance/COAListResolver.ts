import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CoaService } from "src/app/finance/coa.service";
import { ICOA } from "src/app/shared/models/finance/coa";

@Injectable({
	providedIn: 'root'
  })
  export class COAListResolver implements Resolve<ICOA[]> {
  
	constructor(private service: CoaService) {}
  
	resolve(): Observable<ICOA[]> {
	   return this.service.getCoaList();
	}
  
  }