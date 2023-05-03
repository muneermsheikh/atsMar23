import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { UserHistoryService } from "../prospectives/user-history.service";
import { IPaginationUserHistory } from "../shared/pagination/paginationUserHistory";
import { userHistoryParams } from "../shared/params/userHistoryParams";


@Injectable({
     providedIn: 'root'
 })
 export class UserHistoriesResolver implements Resolve<IPaginationUserHistory> {
 
     constructor(private service: UserHistoryService) { }
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPaginationUserHistory> {
      
      var id = +route.paramMap.get('id');  
      var oParams = new userHistoryParams();
      oParams.userHistoryHeaderId=id;
      this.service.setParams(oParams);
      
        return this.service.getUserHistories(false);
     }
 
 }