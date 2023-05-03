import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IContactResult } from "../shared/models/admin/contactResult";
import { CandidateHistoryService } from "../candidates/candidate-history.service";


@Injectable({
     providedIn: 'root'
 })
 export class ContactResultsResolver implements Resolve<IContactResult[]> {
 
     constructor(private candidateHistoryService: CandidateHistoryService) {}
 
     resolve(): Observable<IContactResult[]> {
        return this.candidateHistoryService.getContactResults();
     }
 
 }