import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { IPendingDebitApprovalDto } from 'src/app/shared/dtos/finance/pendingDebitApprovalDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { ConfirmReceiptsService } from '../confirm-receipts.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { MessageService } from 'src/app/shared/services/message.service';
import { IUpdatePaymentConfirmationDto, UpdatePaymentConfirmationDto } from 'src/app/shared/dtos/finance/updatePaymentConfirmationDto';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { message } from 'src/app/shared/models/admin/message';

@Component({
  selector: 'app-confirm-receipts',
  templateUrl: './confirm-receipts.component.html',
  styleUrls: ['./confirm-receipts.component.css']
})
export class ConfirmReceiptsComponent implements OnInit {

  awaiting: IPendingDebitApprovalDto[]=[];
  user?: IUser;
  bolNavigationExtras:boolean=false;
  returnUrl: string='';

  drEntryReviewedOn: Date=new Date();
  drEntryApproved: boolean=false;

  form: FormGroup= new FormGroup({});
  bsValueDate = new Date();

  MESSAGE_TYPEID_REQUEST_PAYMENT_CONFIRMATION=24;
  MESSAGE_TYPEID_CONFIRM_PAYMENT_RECEIVED=25;
  MESSAGE_TYPEID_CONFIRM_PAYMENT_NOT_RECEIVED=26;

  MESSAGE_POST_ACTION=5;  //compose and send Msg;  then send push notification

  constructor(private service: ConfirmReceiptsService,
    private toastr: ToastrService,
    private router: Router,
    private confirmService: ConfirmService,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder,
    //private pushNotice: PushNotificationService,
    private msgService: MessageService) {
      let nav: Navigation | null= this.router.getCurrentNavigation();

        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if(nav.extras.state.userobject) {
              this.user = nav.extras.state.userobject as IUser;
            }
        }
     }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe(data => { 
      this.awaiting = data.confirmationsPending;
    
    })

    this.createForm();
  }

  createForm() {
    this.form = this.fb.group({
      id: 0,
      pending: this.fb.array([])
      
    } 
    );

    this.editAwaiting();
  }

  editAwaiting() {
     this.form.setControl('pending', this.setExistingPending(this.awaiting));
  }

  setExistingPending(ps: IPendingDebitApprovalDto[]) : FormArray {
    const formArray = new FormArray([]);

    ps.forEach(p => {
      formArray.push(this.fb.group({
          voucherEntryId: p.voucherEntryId,
          voucherNo: p.voucherNo,
          voucherDated: p.voucherDated,
          drAccountId: p.drAccountId,
          drAccountName: p.drAccountName,
          drAmount: p.drAmount,
          drEntryApproved: p.drEntryApproved,
          drEntryApprovedOn: this.bsValueDate,
          drEntryApprovedByEmployeeId: p.drEntryApprovedByEmployeeId
      }))
    });

    return formArray;
  }

  get pending(): FormArray {
    return this.form.get("pending") as FormArray
  }

  reviewDateChanged(id: number) {
    var index = this.awaiting.findIndex(x => x.voucherEntryId===id);
    if(index >=0) {
      this.awaiting[index].drEntryApprovedOn=this.drEntryReviewedOn;
      this.awaiting[index].drEntryApproved=true;
    }
  }

  ApprovalChanged(id: number) {
    /*
    var index = this.awaiting.findIndex(x => x.voucherEntryId===id);
    if(index >=0) {
      this.awaiting[index].drEntryApproved=this.drEntryApproved;
      this.awaiting[index].drEntryReviewed=true;
    }
*/
  }

  update() {
    var updatedtos:IUpdatePaymentConfirmationDto[]=[];
    
    for(var i: number=0; i<this.pending.length; i++) {
      var item=this.pending.at(i);
      if(item.get("drEntryApproved")?.value===true) {
        var updatedto= new UpdatePaymentConfirmationDto();
        updatedto.voucherEntryId=item.get("voucherEntryId")?.value;
        updatedto.drEntryApproved=true;
        updatedto.drEntryApprovedOn=new Date();
        updatedto.drEntryApprovedByEmployeeId=this.user?.loggedInEmployeeId!;
        
        updatedtos.push(updatedto);
      
      };
    }
    
    if(updatedtos.length > 0) {
      console.log('updated dtos:', updatedtos);
      this.service.updatePaymentReceipts(updatedtos).subscribe(response => {
        if(response) {
          this.toastr.success('selected payments confirmed as received');
        } else {
          this.toastr.warning('failed to register the payment confirmations');
          return;
        }
      }, error => {
        this.toastr.error('failed to register the payment confirmations');
        return;
      })
    }

    /*updatedtos.forEach(y => {

      var index=this.awaiting.findIndex(x => x.voucherEntryId==y.voucherEntryId);

      console.log('index to splice:', index);
          if(index!==-1) {
            var item = this.awaiting[index];
            var msg = new message();
            msg.bccEmailAddress
            var msg = this.msgService. .composeMessageObject('finance', this.MESSAGE_TYPEID_CONFIRM_PAYMENT_RECEIVED, 
              this.user?.loggedInEmployeeId, item.confirmationRequestedByEmployeeId,
              'payment confirmation', 'receipt of payment confirmed', this.MESSAGE_POST_ACTION);

            this.msgService.sendMessage(msg).subscribe(response => {

            })

            this.awaiting = this.awaiting.splice(index,1);
            this.awaiting.findIndex(x => x.voucherEntryId==y.voucherEntryId);
        }
      }
  )
      */

}

  close() {
    
  }

}
