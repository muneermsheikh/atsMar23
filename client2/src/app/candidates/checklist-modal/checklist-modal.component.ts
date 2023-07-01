import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IChecklistHRDto } from 'src/app/shared/dtos/hr/checklistHRDto';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-checklist-modal',
  templateUrl: './checklist-modal.component.html',
  styleUrls: ['./checklist-modal.component.css']
})
export class ChecklistModalComponent implements OnInit {

  @Input() updateChecklist = new EventEmitter();
  
  chklst?: IChecklistHRDto;
  
  //checklistedOk=false;

  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
      private confirmService: ConfirmService) { }

  ngOnInit(): void {

  }

  updatechecklist() {

    if(this.chklst?.exceptionApproved && (this.chklst.exceptionApprovedBy==='' || this.chklst.exceptionApprovedOn.getFullYear() < 2000)) {
      this.toastr.warning('exception approved must accompany exception made by and approved on');
      return;
    }
    var nonMatching = this.chklst?.checklistHRItems.filter(x => x.mandatoryTrue && !x.accepts).map(x => x.parameter).join(', ');
    if (nonMatching!=='') {
      this.confirmService.confirm("Mandatory parameters not accepted", "Following mandatory parameters not accepted" + nonMatching,
        "Update as it is", "Cancel and edit response").subscribe(result => {
          if (result) {
            this.chklst!.checklistedOk=false;
            this.chklst!.checkedOn=new Date();
            this.updateChecklist.emit(this.chklst);
            this.bsModalRef.hide();
          } else {
            return;
          }
        })
    } else {
      this.chklst!.checklistedOk=true;
      this.chklst!.checkedOn=new Date();
      this.updateChecklist.emit(this.chklst);
      this.bsModalRef.hide();
    }
    return;
  }

  ChargesSingleClicked() {
    console.log('single click');
  }

  ChargesDoubleClicked() {
    console.log('doube cicked, value:', this.chklst?.charges, 'cagreed', this.chklst?.chargesAgreed);
    if(this.chklst?.chargesAgreed === 0) {
      this.chklst.chargesAgreed=this.chklst.charges;
    }
  }

  checkednoerror() {
    if(this.chklst?.exceptionApproved && (this.chklst.exceptionApprovedBy==='' || this.chklst.exceptionApprovedOn.getFullYear() < 2000)) {
      this.toastr.warning('exception approved must accompany exception made by and approved on');
      return false;
    }
    var nonMatching = this.chklst?.checklistHRItems.filter(x => x.mandatoryTrue && !x.response).map(x => x.parameter).join(', ');
    if (nonMatching!=='') {
      this.confirmService.confirm("Mandatory parameters not accepted", "Following mandatory parameters not accepted" + nonMatching,
        "Update as it is", "Cancel and edit response").subscribe(result => {
          if (result) {
            this.chklst!.checklistedOk=false;
            this.chklst!.checkedOn=new Date();
            console.log('result is:', result);
            return true;
          } else {
            console.log('result is:', result);
            return false;
          }
        })
      
      //this.toastr.warning('error - flg Mandatory parameters have not been accepted :', nonMatching );
      //return false;
    } else {
      this.chklst!.checklistedOk=true;
      this.chklst!.checkedOn=new Date();
    }
    return true;
  }


}
