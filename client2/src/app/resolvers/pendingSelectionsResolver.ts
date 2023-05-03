import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { SelectionService } from "../admin/selection.service";
import { IPagination } from "../shared/models/pagination";
import { ISelPendingDto } from "../shared/dtos/admin/selPendingDto";

@Injectable({
     providedIn: 'root'
 })
 export class PendingSelectionsResolver implements Resolve<IPagination<ISelPendingDto[]> | undefined> {
 
     constructor(private selservice: SelectionService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPagination<ISelPendingDto[]> | undefined>{
        return this.selservice.getPendingSelections(false);
     }
 
 }