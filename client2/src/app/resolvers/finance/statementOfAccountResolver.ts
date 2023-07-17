import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { VouchersService } from "../../finance/vouchers.service";
import { IStatementofAccountDto } from "../../shared/dtos/finance/statementOfAccountDto";


@Injectable({
     providedIn: 'root'
 })
 export class StatementOfAccountResolver implements Resolve<IStatementofAccountDto | undefined> {
 
     constructor(private service: VouchersService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IStatementofAccountDto | undefined> {
        
        var id = route.paramMap.get('id');
        if(id==='') return of(undefined);
        
        var fromdate = route.paramMap.get('fromDate') || undefined;
        var uptodate = route.paramMap.get('uptoDate');

        if(fromdate==='' || uptodate==='') return of(undefined);

        var dt1 = new Date(fromdate!);
        var dt2 = new Date(uptodate!);
        
        console.log('reached resolver', dt1.toDateString(), dt2.toDateString());
        if(dt1 !== undefined && dt2 !== undefined) {
            return this.service.getStatementOfAccount(+id!, dt1.toDateString(), dt2.toDateString());
        } else {
         return of(undefined);
        }
        
     }
 
     convertStringToDate(dateStr: string): Date{
        // dateStr = '20200408';
        let year = dateStr.slice(0,4);
        let month = dateStr.slice(4, 6);
        let day = dateStr.slice(6, 8);

        let date = new Date(+year, +month, +day);

        return date;
     }
 }