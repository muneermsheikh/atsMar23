import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { VouchersService } from "../finance/vouchers.service";
import { IFinanceVoucher } from "../shared/models/finance/financeVoucher";



@Injectable({
	providedIn: 'root'
  })
  export class FinanceVoucherResolver implements Resolve<IFinanceVoucher> {
  
	constructor(private service: VouchersService) {}
  
	resolve(route: ActivatedRouteSnapshot): Observable<IFinanceVoucher> {
		
		var id = route.paramMap.get('id');
		//console.log('voucherresolver: ', id);
		
	   	return this.service.getVoucherFromId(+id);
	}
  
  }