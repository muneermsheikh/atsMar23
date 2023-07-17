import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { IStatementofAccountDto, IStatementofAccountItemDto } from 'src/app/shared/dtos/finance/statementOfAccountDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICOA } from 'src/app/shared/models/finance/coa';
import { CoaService } from '../coa.service';
import { VouchersService } from '../vouchers.service';
import { catchError, debounceTime, switchMap, take } from 'rxjs/operators';
import { IVoucherToAddNewPaymentDto } from 'src/app/shared/dtos/finance/voucherToAddNewPaymentDto';
import { AddPaymentModalComponent } from '../add-payment-modal/add-payment-modal.component';
import { coaParams } from 'src/app/shared/params/finance/coaParams';
import { coaDto } from 'src/app/shared/dtos/finance/coaDto';

@Component({
  selector: 'app-finance-index',
  templateUrl: './finance-index.component.html',
  styleUrls: ['./finance-index.component.css']
})
export class FinanceIndexComponent implements OnInit {

  user?: IUser;
  passportno: string='';
  applicationno: number=0;
  mobileno: string='';
  
  //coaparams= new coaParamsFind();
  coaparams= new coaParams();
  coas: ICOA[]=[];
  coa?: ICOA;

  count=0;

  candidateCOA = new coaDto();
  
  candidateAccountNam: string='';
  candidateAccountbal: string='';

  soas: IStatementofAccountItemDto[]=[];
  soa: IStatementofAccountDto|undefined;
  coaSelected: number=0;

  thisPeriodTotalDr: number=0;
  thisPeriodTotalCr: number=0;
  thisPeriodDiff: number=0;
  thisPeriodBalance: number=0;
  thisPeriodSuffix: string='';
  overallBal: number=0;
  overallTotalDr: number=0;
  overallTotalCr: number=0;
  overallBalSuffix: string='';
  candidateAccountClBalance: number=0;
  suffix: any='';

  defaultBankAccount = 30;  //SBI Kurla 

  displaySOA:boolean=false;
  bsModalRef?: BsModalRef;

  constructor(private router: Router, 
      private accountService: AccountService, 
      private toastr: ToastrService,
      private service: CoaService,
      private voucherService: VouchersService,
      private modalService: BsModalService) { 
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!)
  }

  ngOnInit(): void {
    /*var cParas = new coaParams();
    cParas.divn="bankandcash";
    this.service.setParams(cParas);
  
    this.getCOAs(false);
    */
  }

  confirmReceipts() {
    let route = '/finance/receiptspendingconfirmation';
    this.router.navigate(
      [route], 
      { state: 
        { 
          userobject: this.user,
          toedit: true, 
          returnUrl: '/finance' 
        } 
      }
    );
  }


  showCandidateAccount() {
    if(this.applicationno===undefined || this.applicationno===0 ) {
        this.toastr.warning("Application No not set");
        return;
    }

    this.service.getCandidateCOAWithClBal(this.applicationno).subscribe(response => {
      if(response === null) {
        this.toastr.warning('no Chart of account exists with the candidate inputs');
        this.candidateAccountNam='';
      } else {
        
        this.candidateCOA = response;

        this.candidateAccountClBalance = response.clBalance;
        this.candidateAccountNam = response.accountName;

        this.candidateAccountClBalance = Math.abs(response.clBalance);
        this.suffix = response.clBalance < 0 ? ' CR' : ' DR';
        this.coaSelected = response.id;
      }
      
    })
      
  }



  close() {

  }

  addNewCOAForTheCandidate() {
    if(this.applicationno===undefined || this.applicationno=== 0) {
      this.toastr.warning("Appliation No not set");
      return;
    }

    this.service.createCandidateCOA(this.applicationno).subscribe(response => {
      this.coa = response;
      if(response === null) {
        this.toastr.warning('failed to get or create Chart of account exists with the candidate inputs');
      }
    })
  }


  showSOA() {
      const today = new Date(Date());
      const lastDate = new Date(Date());
      lastDate.setFullYear(lastDate.getFullYear()-4);
      var d1=today.toISOString();
      var d2=lastDate.toISOString();
      
          
      this.voucherService.getStatementOfAccount(this.coaSelected, d2, d1)
        .subscribe(response => {
          if(response==null) {
              this.toastr.warning('this account does not exist');
          } else {
            this.soa=response;

            this.thisPeriodTotalDr = this.soa.statementOfAccountItems.map(x => x.dr).reduce((a:number,b:number) => a+b,0);
            this.thisPeriodTotalCr = this.soa.statementOfAccountItems.map(x => x.cr).reduce((a:number,b:number) => a+b,0);
            this.thisPeriodDiff = this.thisPeriodTotalDr - this.thisPeriodTotalCr;
            this.thisPeriodBalance = this.thisPeriodTotalDr-this.thisPeriodTotalCr ; //bcz thisPeridoTotalCr is negative
            this.thisPeriodSuffix = this.thisPeriodBalance > 0 ? ' DR' : ' CR';
            
            //this.overallTotalCr = this.thisPeriodTotalCr + this.soa.opBalance > 0 ? this.soa.opBalance : 0;
            //this.overallTotalDr = this.thisPeriodTotalDr + this.soa.opBalance <= 0 ? this.soa.opBalance : 0;
            if(this.soa.opBalance > 0) {
              this.overallTotalCr = +this.thisPeriodTotalCr +this.soa.opBalance; 
              this.overallTotalDr = this.thisPeriodTotalDr;
            } else {
              this.overallTotalCr = +this.thisPeriodTotalCr;
              this.overallTotalDr = this.thisPeriodTotalDr + this.soa.opBalance;
            }

            this.overallBal = this.overallTotalDr - this.overallTotalCr + this.soa.opBalance;  //bcz ovrallTotalCr is negative
            //console.log('thisperiodtotalcr',this.thisPeriodTotalCr, 'opbal',this.soa.opBalance);
            //console.log('thisperiodtotalDr', this.thisPeriodTotalDr, 'opBal', this.soa.opBalance);
            this.overallBalSuffix = this.overallBal > 0 ? ' DR' : ' CR';
            this.overallBal=Math.abs(this.overallBal);
            this.thisPeriodBalance=Math.abs(this.thisPeriodBalance);
            
            this.displaySOA=true;
            
          }
        })
      
      
      /*
      let route = '/finance/statementofaccount/' + this.coaSelected + '/' + today.toISOString() + '/' + lastDate.toISOString();
      this.router.navigate(
          [route], 
          { state: 
            { 
              userobject: this.user,
              dateRange: {fromDate: today, uptoDate: lastDate},
              returnUrl: '/finance' 
            } }
        );
      */
  }

  toggleDisplaySOA(){
    this.displaySOA = !this.displaySOA;
  }

  showVoucherList() {
    let route = '/finance/voucherlist';
    this.router.navigate(
      [route], 
      { state: 
        { 
          userobject: this.user,
          toedit: true, 
          returnUrl: '/finance' 
        } 
      }
    );
  }

  getCOAs(useCache: boolean) {
    this.service.getCoas(useCache).subscribe({
      next: response => {
        this.coas= response.data;
        this.count = response.count;
        console.log('coas', this.coas);
      },
      error: err => console.log(err)
    })
  };

  
  addNewPaymentReceiptModal() {
    if(this.candidateCOA===undefined) {
      this.toastr.warning("candidate account not selected");
      return;
    }
    
    const config = {
      class:'modal-dialog-centered modal-md',
      initialState: {
        title: 'accept new payment from a candidate',
        accountName: this.candidateCOA.accountName,
        creditAccount: this.candidateCOA.id,
        currentBalance: this.candidateAccountClBalance,
        paymentAmount: this.candidateAccountClBalance
      }
    };
    

    this.bsModalRef = this.modalService.show(AddPaymentModalComponent, config);

    this.bsModalRef.content.paymentVoucherEvent
      .pipe(
        // Switch to load comments
        debounceTime(500),
        catchError((err) => {
          this.toastr.error('failed to get voucher from modal service' + err);
          return of(undefined);
        }),
        switchMap((newVoucher: IVoucherToAddNewPaymentDto) => 
          this.voucherService.insertVoucher(newVoucher)),
        catchError((err) => {
          this.toastr.error('failed to save the payment voucher in the database');
          return of(undefined);
        }
          )
      )
      .subscribe((result: any) => {
        // Finally get the result and show to page
        if(result.body?.returnInt > 0) {
          this.toastr.success('payment voucher created, with Voucher No.: ' + result.body?.returnInt);
        } else {
          this.toastr.warning('failed to create the payment voucher, error:' + result.body?.errorMessage);
        }
        //console.log('resut from API voucher insert', result);
      });


  }

  addNewFinanceTransaction() {
    let route = '/finance/addvoucher';
    this.router.navigate([route], { state: { toedit: true, returnUrl: '/finance' } });
  }

  display(id: number){

  }

  showCOAList() {
    let route = '/finance/coalist';
    this.router.navigate(
      [route], 
      { state: 
        { 
          userobject: this.user,
          toedit: true, 
          returnUrl: '/finance' 
        } 
      }
    );
  }

  onCOASelected() {

  }

  addNewPayment() {
    
  }

  onPageChanged($event: any) {
    const params = this.service.getParams();
    if (params.pageNumber !== $event) {
      params.pageNumber = $event;
      this.service.setParams(params);
      this.getCOAs(true);

    }
  }
}
