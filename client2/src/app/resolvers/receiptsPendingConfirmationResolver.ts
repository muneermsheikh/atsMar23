import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ConfirmReceiptsService } from "../finance/confirm-receipts.service";
import { IPendingDebitApprovalDto } from "../shared/dtos/pendingDebitApprovalDto";

@Injectable({
     providedIn: 'root'
 })
 export class ReceiptsPendingConfirmtionResolver implements Resolve<IPendingDebitApprovalDto[]> {
 
     constructor(private service: ConfirmReceiptsService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPendingDebitApprovalDto[]> {

         return this.service.getPendingConfirmations();
         
     }
 
 }