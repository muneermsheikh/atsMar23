import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { CoaService } from "src/app/finance/coa.service";
import { ICOA } from "src/app/shared/models/finance/coa";
import { IPagination } from "src/app/shared/models/pagination";

@Injectable({
	providedIn: 'root'
  })
  export class COAsPagedResolver implements Resolve<IPagination<ICOA[]>> {
  
	constructor(private service: CoaService) {}
  
	resolve(): Observable<IPagination<ICOA[]>> {
	   return this.service.getCoas(false);
	}
  
  }