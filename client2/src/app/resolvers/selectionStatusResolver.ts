import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { SelectionService } from "../admin/selection.service";
import { ISelectionStatus } from "../shared/models/admin/selectionStatus";


@Injectable({
     providedIn: 'root'
 })
 export class SelectionStatusResolver implements Resolve<ISelectionStatus[]> {
 
     constructor(private selectionService: SelectionService) {}
 
     resolve(): Observable<ISelectionStatus[]> {
        return this.selectionService.getSelectionStatus();
     }

 }