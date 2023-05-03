import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { VouchersService } from "../finance/vouchers.service";
import { IPaginationFinanceVouchers } from "../shared/pagination/paginationFinanceVouchers";



@Injectable({
	providedIn: 'root'
  })
  export class FinanceVouchersResolver implements Resolve<IPaginationFinanceVouchers> {
  
	constructor(private service: VouchersService) {}
  
	resolve(): Observable<IPaginationFinanceVouchers> {

	   return this.service.getVouchers(false);
	}
  
  }