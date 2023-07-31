import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IUserHistoryItem, UserHistoryItem } from 'src/app/shared/models/admin/userHistoryItem';
import { CallRecordsService } from '../call-records.service';
import { IUser } from 'src/app/shared/models/admin/user';
import { IUserHistory } from 'src/app/shared/models/admin/userHistory';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-call-modal',
  templateUrl: './call-modal.component.html',
  styleUrls: ['./call-modal.component.css']
})
export class CallModalComponent implements OnInit {

  @Input() callPartyId = new EventEmitter();
  bAddNew: boolean=false;
  
  candidateName: string='';

  candidateId: number=0;
  cHistory?: IUserHistory;
  personId = 0;
  historyId = 0;
  cHistItem?: IUserHistoryItem;

  candidateFromDb: boolean = true;
  bShowData: boolean=false;
  err: string = '';
  user?: IUser;
  bsValueDate: Date = new Date();
  
  constructor(
    public bsModalRef: BsModalRef, 
    private service: CallRecordsService, 
    private toastr: ToastrService,
    private confirmService: ConfirmService ) {

     }

  ngOnInit(): void {
    this.getHistory();
  }

  getHistory() {
    this.service.getHistoryWithItems(this.historyId).subscribe({
      next: (response: IUserHistory)=> this.cHistory = response,
      error: error => this.toastr.error('Error in modal getting api values')
    })

  }


  close() {
    this.bsModalRef.hide();
  }

  cancelEmit() {
    //this.cBrief=undefined;
  }

  emitValue() {
    if (this.cHistory == null) {
      
      return;
    }
    this.callPartyId.emit(this.cHistory);
    this.bsModalRef.hide();
  }

  editHistoryItem(item: IUserHistoryItem) {
    this.cHistItem = item;
   
    this.bAddNew=true;
  }

  deleteHistoryItem(item: IUserHistoryItem) {
      var response = this.confirmService.confirm("Confirm Delete Transaction", 
      "Press 'Delete' to delete following transaction:'" + item.gistOfDiscussions.substring(0,15) + '...',
      'Delete', 'Cancel').subscribe({
        next: response => {
          if(response) {
            this.service.deleteHistoryItem(item.id).subscribe({
              next: response => {
                if(response) {
                  this.toastr.success('record successfully deleted');
                  //delete from the array
                  var index = this.cHistory?.userHistoryItems.findIndex(x => x.id===item.id);
                  if(index !== undefined) this.cHistory?.userHistoryItems.splice(index, 1);
                } else {
                  this.toastr.info('Failed to delete the record');
                }
              }
            })
          } else {
            this.toastr.info('deletion aborted');
          }
        },
        error: error => this.toastr.error('Error encountered', error)
      });
  }

  addNewItem() {
    this.cHistItem = new UserHistoryItem();
    this.cHistItem.loggedInUserId=this.user?.loggedInEmployeeId!;
    this.cHistItem.loggedInUserName=this.user?.displayName!;
    this.cHistItem.dateOfContact = new Date();
    this.cHistItem.subject = this.cHistory?.userHistoryItems.length 
      ? this.cHistory.userHistoryItems[this.cHistory.userHistoryItems.length]?.subject :'';
    this.cHistItem.userHistoryId = this.cHistory?.id!;
    this.cHistItem.personId = this.personId;
    
    this.bAddNew=true;
    
  }

  updateRecord() {
    
    if(this.cHistItem !== undefined ) {
      this.service.updateHistoryItem(this.cHistItem).subscribe({
        next: response => {
          if(response !== null) {
            this.toastr.success('record updated');
            if(this.cHistItem?.id==0) this.cHistory?.userHistoryItems.push(response);
            this.bAddNew=false;
          } else {
            this.toastr.info('failed to insert the record');
          }
        },
        error: error => this.toastr.error('Error:', error)
      });
  
    }
  }

  cancelNewRecord() {
    this.bsModalRef.hide();
  }

}
