import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { VouchersService } from "../finance/vouchers.service";
import { IStatementofAccountDto } from "../shared/dtos/statementOfAccountDto";

@Injectable({
     providedIn: 'root'
 })
 export class StatementOfAccountResolver implements Resolve<IStatementofAccountDto> {
 
     constructor(private service: VouchersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IStatementofAccountDto> {
        

        var id = route.paramMap.get('id');
        var fromdate = route.paramMap.get('fromDate');
        var uptodate = route.paramMap.get('uptoDate');
        if(fromdate==='' || uptodate==='') return null;

        var dt1 = new Date(fromdate);
        var dt2 = new Date(uptodate);
        var d1 = new Date(dt1.getFullYear(), dt1.getMonth(), dt1.getDate());
        var d2 = new Date(dt2.getFullYear(), dt2.getMonth(), dt2.getDate());
        
        if(id==='') return null;
        console.log('reached resolver', d1, d2);
        return this.service.getStatementOfAccount(+id, d1.toISOString(), d2.toISOString());
     }
 
 }