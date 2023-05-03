import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ClientReviewService } from "../client/client-review.service";
import { ICustomerReviewData } from "../shared/models/customerReviewData";

@Injectable({
     providedIn: 'root'
 })
 export class CustomerReviewStatusDataResolver implements Resolve<ICustomerReviewData[]> {
 
     constructor(private service: ClientReviewService) {}
 
     resolve(): Observable<ICustomerReviewData[]> {
        return this.service.getCustomerReviewStatusData();
     }
 
 }