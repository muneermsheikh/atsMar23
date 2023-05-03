import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ReviewService } from "../orders/review.service";
import { IContractReview } from "../shared/models/contractReview";

@Injectable({
     providedIn: 'root'
 })
 
 export class ContractReviewResolver implements Resolve<IContractReview[]> {
 
     constructor(private service: ReviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IContractReview[]> {
        return this.service.getReview(+route.paramMap.get('id'));
     }
 
 }