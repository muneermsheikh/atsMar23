import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { IUserHistoryDto } from 'src/app/shared/dtos/admin/userHistoryDto';
import { CallModalComponent } from '../call-modal/call-modal.component';

@Component({
  selector: 'app-call-list',
  templateUrl: './call-list.component.html',
  styleUrls: ['./call-list.component.css']
})
export class CallListComponent implements OnInit {

  bsModalRef?: BsModalRef;
  cBrief?: ICandidateBriefDto;
  cHistory?: IUserHistoryDto;
  constructor(private modalService: BsModalService, private router:Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.openCallRecordModal();
  }

  openCallRecordModal() {
      const config = {
        class: 'modal-dialog-centered modal-lg',
        cBrief: this.cBrief,
        candidateFromDb: false
      }

      this.bsModalRef = this.modalService.show(CallModalComponent, config);
      this.bsModalRef.content.callPartyId.subscribe((values: IUserHistoryDto) => {
        this.cHistory = values;
        console.log('opencallrecordmodal, cHistory returned from modal is: ', this.cHistory);
        if (this.cHistory===null) {
            this.toastr.warning('Your inputs did not return any history');
            return;
        }
        this.router.navigateByUrl('/candidate/history/' + this.cHistory.id );
        
      }, (error: any) => {
        this.toastr.error(error);
      })
  } 

}
