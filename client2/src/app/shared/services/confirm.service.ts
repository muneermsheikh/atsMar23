import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IHelp } from '../models/admin/help';
import { ConfirmModalComponent } from '../components/modal/confirm-modal/confirm-modal.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  baseUrl = environment.apiUrl;
  
  bsModelRef: BsModalRef | undefined;

  constructor(private modalService: BsModalService, private http: HttpClient) { }

  confirm(title = 'Confirmation', 
    message = 'Are you sure you want to do this?', 
    btnOkText = 'Ok', 
    btnCancelText = 'Cancel'): Observable<boolean> {
      const config = {
        initialState: {
          title, 
          message,
          btnOkText,
          btnCancelText
        }
      }
    this.bsModelRef = this.modalService.show(ConfirmModalComponent, config);
    
    return new Observable<boolean>(this.getResult());
  }

  private getResult() {
    return (observer: any) => {
      const subscription = this.bsModelRef!.onHidden!.subscribe(() => {
        observer.next(this.bsModelRef!.content.result);
        observer.complete();
      });

      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }

  getHelp(topic: string)
  {
    return this.http.get<IHelp>(this.baseUrl + 'masters/help/' + topic);
  }
}