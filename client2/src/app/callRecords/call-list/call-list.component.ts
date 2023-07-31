import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { IUserHistoryDto } from 'src/app/shared/dtos/admin/userHistoryDto';
import { CallModalComponent } from '../call-modal/call-modal.component';
import { userHistoryParams } from 'src/app/shared/params/admin/userHistoryParams';
import { CallRecordsService } from '../call-records.service';
import { IUserHistory } from 'src/app/shared/models/admin/userHistory';
import { take } from 'rxjs/operators';
import { IUser } from 'src/app/shared/models/admin/user';
import { AccountService } from 'src/app/account/account.service';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-call-list',
  templateUrl: './call-list.component.html',
  styleUrls: ['./call-list.component.css']
})
export class CallListComponent implements OnInit {

  params= new userHistoryParams();
  
  bsModalRef?: BsModalRef;
  cBrief?: ICandidateBriefDto;
  cHistories: IUserHistoryDto[]=[];
  hist? : IUserHistory;

  user?: IUser;
  returnUrl = '';

  constructor(
    private callrecordsservice: CallRecordsService,
    private modalService: BsModalService, 
    private router:Router, 
    private toastr: ToastrService,
    private confirmService: ConfirmService,
    private accountsService: AccountService) { 
      //let nav: Navigation|null = this.router.getCurrentNavigation() ;
      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
        /* if (nav?.extras && nav.extras.state) {
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if( nav.extras.state.user) {
              this.user = nav.extras.state.user as IUser;
              //this.hasEditRole = this.user.roles.includes('AdminManager');
              //this.hasHRRole =this.user.roles.includes('HRSupervisor');
            }
        }
        this.bcService.set('@callRecordsList',' ');
        */
    }

  ngOnInit(): void {
      var hParams = new userHistoryParams();

      hParams.userName=this.user?.displayName!;
      hParams.status="active";
      this.callrecordsservice.setParams(hParams);
      
      this.callrecordsservice.getHistories(false).subscribe(response => {
        this.cHistories = response.data;
      })
      //console.log('histories:', this.cHistories);
  }

  deleteHistory(historyId: any) {
    this.confirmService.confirm('confirm delete the History Object', 'This will Delete The Parent History Object alongwith all its children',
      'DELETE whole object', 'Abort').subscribe({
        next: confirmed => {
          if(confirmed) {
            this.callrecordsservice.deleteHistory(historyId).subscribe({
              next: deleted => {
                if(deleted) {
                  this.toastr.success('deleted User History');
                  //update DOM
                } else {
                  this.toastr.warning('failed to delete the User History');
                }
              },
              error: error => this.toastr.error('Error in deleting the History Object')
            });
          } else {
            this.toastr.info('Delete aborted');
          }
        },
        error: error => this.toastr.error('Error in getting delete confirmation')
      });
  }


  editHistoryModal(dto: IUserHistoryDto) {
    
      const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          candidateName: dto.name,
          personId: dto.personId,
          historyId: dto.id,
          candidateFromDb: false
        }
      }

      this.bsModalRef = this.modalService.show(CallModalComponent, config);

      this.bsModalRef.content.callPartyId.subscribe(() => {
        
      })
  } 


  buttonCancel() {
    
  }

  buttonOk() {

    this.callrecordsservice.setParams(this.params);  
    this.callrecordsservice.getOrcreateNewHistory().subscribe({
      next: (response: IUserHistoryDto) => {
        if(response !== undefined && response !== null) {
          if(response.errorMessage !=='') {
            this.toastr.warning(response.errorMessage, 'Error encountered');
            console.log(response.errorMessage);
            this.cHistories = [];
          } else {
            this.cHistories = [];
            this.cHistories.push(response);
          }
        }
      },
      error: error => this.toastr.error('Error in getting records from server:', error)
    })
  }

  navigateByRoute(id: number, routeString: string, editable: boolean) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/calllist' 
          } }
      );
  }
}
