import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IChecklistHRDto } from '../shared/dtos/hr/checklistHRDto';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ChecklistService {

  apiUrl = environment.apiUrl;
  //private currentUserSource = new ReplaySubject<IUser>(1);
  //currentUser$ = this.currentUserSource.asObservable();
  cl?: IChecklistHRDto;
  
  private checklistSource = new BehaviorSubject<IChecklistHRDto|null>(null) ;
  checklist$ = this.checklistSource.asObservable();

  constructor(private http: HttpClient) { }

  getChecklist(candidateid: number, orderitemid: number) {
    return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/checklisthr/' + candidateid  + '/' + orderitemid);
  }

  updateChecklist(checklist: IChecklistHRDto) {
    //console.log(checklist);
    return this.http.put(this.apiUrl + 'checklist/checklisthr', checklist);
  }

}
