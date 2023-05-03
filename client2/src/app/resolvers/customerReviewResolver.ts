import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ClientReviewService } from "../client/client-review.service";
import { ICustomerReview } from "../shared/models/customerReview";

@Injectable({
     providedIn: 'root'
 })
 export class CustomerReviewResolver implements Resolve<ICustomerReview> {
 
     constructor(private service: ClientReviewService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<ICustomerReview> {
        return this.service.getCustomerReview(+route.paramMap.get('id'));
     }
 
 }