import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IVoucherToAddNewPaymentDto, VoucherToAddNewPaymentDto } from 'src/app/shared/dtos/finance/voucherToAddNewPaymentDto';
import { ICOA } from 'src/app/shared/models/masters/finance/coa';
import { CoaService } from '../coa.service';

@Component({
  selector: 'app-add-payment-modal',
  templateUrl: './add-payment-modal.component.html',
  styleUrls: ['./add-payment-modal.component.css']
})
export class AddPaymentModalComponent implements OnInit {

  @Output() paymentVoucherEvent = new EventEmitter<IVoucherToAddNewPaymentDto|null>();
  
  title: string='';
  accountName: string='';
  
  currentBalance: number=0;

  paymentAmount: number=0;
  bankAndCashAccounts: ICOA[]=[];
  creditAccount: number=0;
  paymentDated: Date = new Date();
  drEntryRequiresApproval: boolean=false;
  narration: string='';

  debitAccountId: number=0;
  bsValue = new Date();

  newVoucher = new VoucherToAddNewPaymentDto()
  
  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService, private service: CoaService) { }

  ngOnInit(): void {
    this.service.getGroupOfCOAs("banks").subscribe({
      next: responses => {
        this.bankAndCashAccounts = responses;
      },
      error: err => this.toastr.error('error in getting cashandbank', err)
    });
  }

  addNewVoucher() {
    if(this.paymentAmount <=0 || this.creditAccount===0 ) {
      this.toastr.warning('invalid inputs');
      return;
    }
    this.newVoucher.debitCOAId=this.debitAccountId;
    this.newVoucher.creditCOAId=this.creditAccount;
    this.newVoucher.amount=+this.paymentAmount;
    this.newVoucher.paymentDate = this.paymentDated;
    this.newVoucher.narration = this.narration;
    this.newVoucher.drEntryRequiresApproval=this.drEntryRequiresApproval;
    this.paymentVoucherEvent.emit(this.newVoucher);
    this.bsModalRef.hide();
  }

  onBankCashAccountChanged() {
    this.drEntryRequiresApproval = this.debitAccountId===2;
  }

  close() {
    this.toastr.warning('aborted');
    console.log('aborted');
    this.paymentVoucherEvent.emit(null);
    this.bsModalRef.hide();
  }
}
