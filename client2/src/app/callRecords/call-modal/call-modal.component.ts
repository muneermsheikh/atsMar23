import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { CandidateHistoryService } from 'src/app/candidates/candidate-history.service';
import { CandidateBriefDto, ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { IUserHistoryDto } from 'src/app/shared/dtos/admin/userHistoryDto';
import { userHistoryParams } from 'src/app/shared/params/admin/userHistoryParams';

@Component({
  selector: 'app-call-modal',
  templateUrl: './call-modal.component.html',
  styleUrls: ['./call-modal.component.css']
})
export class CallModalComponent implements OnInit {

  @Input() callPartyId = new EventEmitter();
  
  candidateId: number=0;
  cBrief?: ICandidateBriefDto;
  cHistory?: IUserHistoryDto;
  //cvParams: candidateParams;
  histParams?: userHistoryParams;
  candidateFromDb: boolean = true;
  bShowData: boolean=false;
  err: string = '';

  constructor(public bsModalRef: BsModalRef, private candidateHistoryService: CandidateHistoryService, private toastr: ToastrService ) { }

  ngOnInit(): void {
    this.cBrief = new CandidateBriefDto();
    //this.cvParams = new candidateParams();
    this.histParams = new userHistoryParams();
  }

  close() {
    this.bsModalRef.hide();
  }

  updateRoles() {
    if (this.cBrief?.fullName === '' || this.cBrief?.mobileNo === '') {
      this.toastr.warning('Name and mobile No mandatory');
      return;
    }
    this.callPartyId.emit(this.cBrief);
    this.bsModalRef.hide();
  }

  getCandidate() {
    this.err='';

      if (this.histParams?.mobileNo==='' && this.histParams.emailId ==='' && this.histParams.personId === 0) {
        this.toastr.warning('no inputs');
        return;
      }

      return this.candidateHistoryService.getHistory(this.histParams!)
          .subscribe((response: any) => {
            if (response === undefined) {
              this.toastr.warning('failed to retrieve history data from api');
              return;
            }
            console.log('response from api ', response);
            this.cHistory = response;
            this.bShowData=this.cHistory?.name !== '';
      }, (error: any) => {
        this.err = error;
        this.toastr.error(error);
      })

}

  cancelEmit() {
    this.cBrief=undefined;
  }

  emitValue() {
    if (this.cHistory == null) {
      
      return;
    }
    this.callPartyId.emit(this.cHistory);
    this.bsModalRef.hide();
  }



}
