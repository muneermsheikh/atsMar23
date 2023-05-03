import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ProspectiveService } from "../prospectives/prospective.service";
import { IProspectiveSummaryDto } from "../shared/dtos/prospectiveSummaryDto";
import { IPaginationProspectiveSummary } from "../shared/pagination/paginationProspectiveSummary";
import { prospectiveSummaryParams } from "../shared/params/prospectiveSummaryParams";


@Injectable({
     providedIn: 'root'
 })
 export class ProspectiveSummaryResolver implements Resolve<IProspectiveSummaryDto[]> {
 
     constructor(private prospectiveService: ProspectiveService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IProspectiveSummaryDto[]> {
        var usecache = route.paramMap.get('useCache');

        var useCache=usecache==='' ? true : Boolean(usecache);

        return this.prospectiveService.getProspectiveSummary(useCache);
     }
 
 }