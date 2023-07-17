import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { VouchersService } from "../../finance/vouchers.service";
import { IFinanceVoucher } from "../../shared/models/finance/financeVoucher";



@Injectable({
	providedIn: 'root'
  })
  export class FinanceVoucherResolver implements Resolve<IFinanceVoucher | any> {
  
	constructor(private service: VouchersService) {}
  
	resolve(route: ActivatedRouteSnapshot): Observable<IFinanceVoucher | any> {
		
		var id = route.paramMap.get('id');
		//console.log('voucherresolver: ', id);
		if(id==='') return of(undefined);

	   	if (id !== undefined && id !== '' && id !== null) {
			return this.service.getVoucherFromId(+id);
		} else {
			return of(undefined);
		}
		
	}
  
  }