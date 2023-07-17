import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IVoucherDto } from 'src/app/shared/dtos/finance/voucherDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { transactionParams } from 'src/app/shared/params/finance/tranactionParams';
import { VouchersService } from '../vouchers.service';
import { ToastrService } from 'ngx-toastr';
import { Navigation, Router } from '@angular/router';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { catchError, filter, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-voucher-list',
  templateUrl: './voucher-list.component.html',
  styleUrls: ['./voucher-list.component.css']
})
export class VoucherListComponent implements OnInit {

  
  @ViewChild('search', {static: false}) searchTerm?: ElementRef;

  sParams = new transactionParams();
  totalCount= 0;
  vouchers: IVoucherDto[]=[];
  voucher?: IVoucherDto;
  
  user?: IUser;
  bolNavigationExtras:boolean=false;
  returnUrl: string='';

  constructor( private service: VouchersService,
      private toastr: ToastrService,
      private router: Router,
      private confirmService: ConfirmService) {

        let nav: Navigation | null = this.router.getCurrentNavigation();

        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if(nav.extras.state.userobject) {
              this.user = nav.extras.state.userobject as IUser;
            }
        }
       }

  ngOnInit(): void {
    this.getVouchers(false);
  }

  getVouchers(useCache=false) {
    return this.service.getVouchers(useCache).subscribe(response =>{
      this.vouchers = response.data;
      this.totalCount=response.count;
    }, error => {
      console.log('error', error);
    })
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new transactionParams();
    this.service.setParams(this.sParams);
    this.getVouchers(true);
  }
  
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setParams(params);
      this.getVouchers(true);

    }
  }

  
  onSearch() {
  }


  addNewFinanceTransaction() {
    let route = '/finance/addvoucher';
    this.router.navigate([route], { state: { toedit: true, returnUrl: '/finance/voucherlist' } });
  }

  editTransaction(id: number, toedit: boolean) {
    let route = '/finance/editfinancewithobject/' + id;
    this.router.navigate([route], { state: { toedit: toedit, returnUrl: '/finance/voucherlist', userObject: this.user } });

  }

  deleteVoucher(voucherid: any) {

    this.confirmService.confirm('confirm delete this voucher', 'confirm delete voucher').pipe(
      filter(result => result),
      switchMap(confirmed => this.service.deleteVoucher(voucherid).pipe(
        catchError(err => {
          console.log('Error in deleting the voucher', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted voucher')),
        //tap(res=>console.log('delete voucher succeeded')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
      deleteReponse => {
        console.log('deete succeeded');
        this.toastr.success('voucher deleted');
      },
      err => {
        console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      }
    )


    /*this.service.deleteVoucher(voucherid).subscribe(response  => {
      if (response) {
        this.removeTaskFromCache(voucherid);
        
      } else {
        this.toastr.error('failed to delete the voucher');
      }
    }, error => {
      this.toastr.error('failed to delete the Voucher', error);
    })
 */

  }


  removeTaskFromCache(id: number) {
    this.service.deleteVoucherFromCache(id);
    var index=this.vouchers.findIndex(x => x.id==id);
    this.vouchers.splice(index,1);
    this.toastr.success('task deleted');
  }

  
  returnToCaller() {
    //console.log('return to caller:', this.returnUrl);
    this.router.navigateByUrl(this.returnUrl || '' );
  }

}
