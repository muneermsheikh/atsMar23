import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable, of } from "rxjs";
import { CallRecordsService } from "../callRecords/call-records.service";
import { IPagination } from "../shared/models/pagination";
import { IUserHistoryDto } from "../shared/dtos/admin/userHistoryDto";
import { userHistoryParams } from "../shared/params/admin/userHistoryParams";
import { AccountService } from "../account/account.service";
import { take } from "rxjs/operators";
import { IUser } from "../shared/models/admin/user";


@Injectable({
     providedIn: 'root'
 })
 export class PendingCallRecordsOfLoggedInUserResolver implements Resolve<IPagination<IUserHistoryDto[]>> {
    user?: IUser;
    
     constructor(private service: CallRecordsService, private accountsService: AccountService) {
        this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
      }
 
     resolve(): Observable<IPagination<IUserHistoryDto[]>> {
      
      console.log('user in resolvers:', this.user);

      if(this.user===null) return of();

        var hParams = new userHistoryParams();
        hParams.userName=this.user?.username!;
        hParams.status="active";
      
        this.service.setParams(hParams);
        console.log('in resolver, params is:', hParams);
        
    return this.service.getHistories(false);
     }
 
 }