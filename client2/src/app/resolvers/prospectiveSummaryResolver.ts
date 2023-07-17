import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ProspectiveService } from "../prospectives/prospective.service";
import { IProspectiveSummaryDto } from "../shared/dtos/hr/propectiveSummaryDto";


@Injectable({
     providedIn: 'root'
 })
 export class ProspectiveSummaryResolver implements Resolve<IProspectiveSummaryDto[]> {
 
     constructor(private prospectiveService: ProspectiveService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IProspectiveSummaryDto[]> {
        /*var usecache = route.paramMap.get('useCache');

        var useCache=usecache==='' ? true : Boolean(usecache);
        console.log('useCache in resolver:', useCache);
        */
        return this.prospectiveService.getProspectiveSummary(false);
     }
 
 }