import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IContractReview } from '../shared/models/admin/contractReview';
import { IReviewItemStatus } from '../shared/models/admin/reviewItemStatus';
import { contractReviewParams } from '../shared/params/admin/contractReviewParams';
import { IPagination } from '../shared/models/pagination';
import { HttpClient, HttpParams } from '@angular/common/http';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IReviewItemData } from '../shared/models/admin/reviewItemData';
import { IContractReviewItem } from '../shared/models/admin/contractReviewItem';
import { IReviewItem } from '../shared/models/admin/reviewItem';
import { IContractReviewItemReturnValueDto } from '../shared/dtos/admin/contractReviewItemReturnValueDto';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  apiUrl = environment.apiUrl;
  review?: IContractReview;
  reviewItemStatuses: IReviewItemStatus[]=[];

  cache = new Map();
  cacheS = new Map();   //for item status values

  oParams = new contractReviewParams();
  pagination: IPagination<IContractReview[]> | undefined;  // = new PaginationContractReview();

  constructor(private http: HttpClient) { }

  getItemReviewStatusName(itemid: number): string {
    if(this.reviewItemStatuses.length===0 ) {
      this.getReviewItemStatuses(false).subscribe((response: any)=> {
        this.reviewItemStatuses=response;
      })
    }
    console.log('itemreviews', this.reviewItemStatuses);
    var dto =this.reviewItemStatuses.filter(x => x.id===itemid).map(x => x.itemStatus);
    return dto[1];
  }

  getReviewItemStatuses(useCache: boolean) {
      if(!useCache) this.cacheS = new Map();
      if(useCache && this.cacheS.size > 0) {
        console.log('found the cache');
        return of(this.reviewItemStatuses);
      }

      console.log('getting reviewitemstatuses from api');
      return this.http.get<IReviewItemStatus[]>(this.apiUrl + 'contractreview/reviewitemstatus');
    }

  getReview(id: number) {
    return this.http.get<IContractReview[]>(this.apiUrl + 'contractreview/' + id);
  }

  getReviewItem(id: number) {
    return this.http.get<IContractReviewItem>(this.apiUrl + 'contractreview/reviewitem/' + id);
  }

  getReviewItems(id: number) {
    return this.http.get<IReviewItem[]>(this.apiUrl + 'contractreview/reviewresult/' + id);
  }

  updateReviewItem(model: IContractReviewItem) 
  {
    return this.http.put<IContractReviewItemReturnValueDto>(this.apiUrl + 'ContractReview/reviewitem', model);
  }

  getReviews(useCache: boolean, id: number) {

    if (useCache === false) {
      this.cache = new Map();
    }

    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.oParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.oParams).join('-'));
        return of(this.pagination);
      }
    }

    let params = new HttpParams();
    if (this.oParams.city !== "") {
      params = params.append('city', this.oParams.city);
    }
    if (this.oParams.categoryId !== 0) {
      params = params.append('categoryId', this.oParams.categoryId.toString());
    }

    //if (this.oParams.orderidInts !=null && this.oParams.orderidInts.length !== 0) {
      //params = params.append('orderids', this.oParams.orderidInts.join(', '));
      params = params.append('orderids', id + ',' + id);
    //}

    if (this.oParams.search) {
      params = params.append('search', this.oParams.search);
    }
    
    params = params.append('sort', this.oParams.sort);
    params = params.append('pageIndex', this.oParams.pageNumber.toString());
    params = params.append('pageSize', this.oParams.pageSize.toString());

    console.log('params', params);

    return this.http.get<IPagination<IContractReview[]>>(this.apiUrl + 'contractreview/reviews', {params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(this.oParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
  }
  
  getReviewData() {
    return this.http.get<IReviewItemData[]>(this.apiUrl + 'contractreview/reviewdata');
  }
  
  register(model: any) {
    return this.http.post(this.apiUrl + 'orders/review', model);
    }
  
  updateReview(model: any) {
    return this.http.put(this.apiUrl + 'orders/review', model)
  }

  updateReviews(model: any[]) {
    return this.http.put(this.apiUrl + 'orders/reviews', model)
  }
}
