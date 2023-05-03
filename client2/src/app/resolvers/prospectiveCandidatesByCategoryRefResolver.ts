import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { ProspectiveService } from "../prospectives/prospective.service";
import { IPaginationProspectiveCandidates } from "../shared/pagination/paginationProspectiveCandidates";
import { prospectiveCandidateParams } from "../shared/params/prospectiveCandidateParams";


@Injectable({
     providedIn: 'root'
 })
 export class ProspectiveCandidatesByCategoryRefResolver implements Resolve<IPaginationProspectiveCandidates> {
 
     constructor(private prospectiveService: ProspectiveService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IPaginationProspectiveCandidates> {

        var routeid = route.paramMap.get('categoryRef');
        var status = route.paramMap.get('status');
        if (routeid === '') return null;
        var pParams = new prospectiveCandidateParams();
        pParams.categoryRef=routeid;
        pParams.categoryRef=routeid;
        pParams.status = status;
        this.prospectiveService.setParams(pParams);
        return this.prospectiveService.getProspectiveCandidates(false);
     }
 
 }