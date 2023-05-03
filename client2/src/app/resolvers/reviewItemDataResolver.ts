import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ReviewService } from "../orders/review.service";
import { IReviewItemData } from "../shared/models/reviewItemData";

@Injectable({
     providedIn: 'root'
 })
 export class ReviewItemDataResolver implements Resolve<IReviewItemData[]> {
 
     constructor(private service: ReviewService) {}
 
     resolve(): Observable<IReviewItemData[]> {
        return this.service.getReviewData();
     }
 
 }