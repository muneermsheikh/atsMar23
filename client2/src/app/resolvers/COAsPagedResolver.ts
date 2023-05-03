import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ChartOfAccountsService } from "../fin/chartOfAccounts.service";
import { PaginationCOA } from "../shared/pagination/paginationCOA";

@Injectable({
	providedIn: 'root'
  })
  export class COAsPagedResolver implements Resolve<PaginationCOA> {
  
	constructor(private service: ChartOfAccountsService) {}
  
	resolve(): Observable<PaginationCOA> {
	   return this.service.getCOAs(false);
	}
  
  }