import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { VouchersService } from "../finance/vouchers.service";
import { IPagination } from "../shared/models/pagination";
import { IFinanceVoucher } from "../shared/models/finance/financeVoucher";



@Injectable({
	providedIn: 'root'
  })
  export class FinanceVouchersResolver implements Resolve<IPagination<IFinanceVoucher[]>> {
  
	constructor(private service: VouchersService) {}
  
	resolve(): Observable<IPagination<IFinanceVoucher[]>> {

	   return this.service.getVouchers(false);
	}
  
  }