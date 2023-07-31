import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICOA, coa } from 'src/app/shared/models/masters/finance/coa';
import { coaParams } from 'src/app/shared/params/finance/coaParams';
import { CoaService } from '../coa.service';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { Navigation, Router } from '@angular/router';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { InputModalComponent } from 'src/app/shared/components/input-modal/input-modal.component';
import { DateInputRangeModalComponent } from 'src/app/shared/components/date-input-range-modal/date-input-range-modal.component';
import { AddPaymentModalComponent } from '../add-payment-modal/add-payment-modal.component';
import { ICustomerOfficial } from 'src/app/shared/models/admin/customerOfficial';
import { map, switchMap } from 'rxjs/operators';
import { CoaEditModalComponent } from '../coa-edit-modal/coa-edit-modal.component';


@Component({
  selector: 'app-coa-list',
  templateUrl: './coa-list.component.html',
  styleUrls: ['./coa-list.component.css']
})
export class CoaListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;

  user?: IUser;
  sParams = new coaParams();
  totalCount: number=0;
  coas: ICOA[]=[];

  bolNavigationExtras:boolean=false;
  returnUrl: string='';
  bsModalRef?: BsModalRef;
  inputApplicationNo=0;

  sortOptions = [
    {name:'By Account Name Asc', value:'name'},
    {name:'By Account Name Desc', value:'namedesc'},
    {name:'By Account Type Asc', value:'type'},
    {name:'By Account Type Desc', value:'typedesc'},
    {name:'By Division Asc', value:'divn'},
    {name:'By Division Desc', value:'divndesc'}
  ]

  constructor( 
      private service: CoaService,
      //private accountService: AccountService, 
      private toastr: ToastrService,
      private router: Router,
      private confirmService: ConfirmService,
      private modalService: BsModalService
      ) {
        //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);

        //read navigationExtras
      let nav: Navigation|null = this.router.getCurrentNavigation();

      if (nav?.extras && nav.extras.state) {
          this.bolNavigationExtras=true;
          if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

          if(nav.extras.state.userobject) {
            this.user = nav.extras.state.userobject as IUser;
          }
      }
    
       }

  async ngOnInit(): Promise<void> {
    await this.getCOAs(false);
  }

  async getCOAs(useCache=false) {

    return this.service.getCoas(useCache).subscribe({
      next: response => {
        this.coas = response.data;
        this.totalCount = response.count;
      },
      error: err => console.log(err)
    });
    
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new coaParams();
    this.service.setParams(this.sParams);
    this.getCOAs();
  }
  
  onPageChanged(event: any){
    
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      console.log('params after api call:', params);
      this.service.setParams(params);
      this.getCOAs(true);
    }
  }
  
  onSearch() {
    var searchString=this.searchTerm!.nativeElement.value;
    if(this.sParams.search===searchString) return;

    this.sParams.search = this.searchTerm!.nativeElement.value;
    this.sParams.pageNumber = 1;
    this.service.setParams(this.sParams);
    this.getCOAs(true);
  }


  addNewCOA() {
    let route = '/finance/addaccount';
    this.router.navigate(
      [route], 
      { state: 
        { 
          coatoedit: undefined, 
          userobject: this.user,
          toedit: false, 
          returnUrl: '/finance/coalist' 
        } 
      }
    );

  }

  editCOAModal(edit: ICOA | null) {
    if(edit===null) edit = new coa();
    
    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        title: 'edit Chart of account',
        coa: edit
      }
    }
    this.bsModalRef = this.modalService.show(CoaEditModalComponent, config);
        
    this.bsModalRef.content.editCOAEvent.subscribe({
      next: (editedCOA: ICOA|null) => {
        console.log('returned from modal', editedCOA);
        if(editedCOA===null) {
          this.toastr.warning('Aborted by user');
        } else {
          this.service.updateCOA(editedCOA).subscribe({
            next: (success: boolean) => {
              if(success) {
                this.toastr.success('Chart of account updated');
              } else {
                this.toastr.warning('Failed to update the Chart of Account');
              }
            },
            error: (error: any) => this.toastr.error('Failed to update the Chart of Account:', error)
          })
        }
      },
      
      error: (error: any) => this.toastr.error('Failed to update the Chart of account:', error)
    })

    /*
    this.bsModalRef.content.editCOAEvent.pipe(
      switchMap((coaedited: ICOA) => {
        
        return this.service.updateCOA(coaedited).pipe(
          map((success: boolean) => {
            if(!success) {
              this.toastr.warning('failed to update the Chart of account');
              return false;
            } else {
              this.toastr.success('chart of account updated');
              return true;
          }
          })
        )
      })
    )
    */
  }
  
  editCoa(t: ICOA) {
    let route = '/finance/editcoawithobject';
    this.router.navigate(
      [route], 
      { state: 
        { 
          coatoedit: t, 
          userobject: this.user,
          toedit: true, 
          returnUrl: '/finance/coalist' 
        } 
      }
    );
  }

  viewCoa(t: ICOA) {
    let route = '/finance/editcoawithobject';
    this.router.navigate(
        [route], 
        { state: 
          { 
            coatoedit: t, 
            userobject: this.user,
            toedit: false, 
            returnUrl: '/finance/coalist' 
          } }
      );
  }

  updateCandidateAccountName(t: ICOA) 
  {
    const config = {
      class:'modal-dialog-centered modal-md',
      initialState: {
        title: 'get Application Number to include in the Account Name'
      }
    };
    
    this.bsModalRef = this.modalService.show(InputModalComponent, config);

    this.bsModalRef.content.returnInputValue.subscribe((applicationno: any) => {
        this.inputApplicationNo = applicationno;
    });

    if(t.accountName.includes('Application')) {
      var pos = t.accountName.indexOf('Application');
      t.accountName = t.accountName.substring(1,pos +10) + this.inputApplicationNo;
    } else {
      t.accountName = t.accountName + '-Application ' + this.inputApplicationNo;
    }

      this.service.editCOA(t).subscribe(response => {
        var index=this.coas.findIndex(x => x.id===t.id);
        if ((index: number) => 0) {
          this.coas[index]=response;
        }
      })
    
  }

  statementOfAccount(accountid: number) {

    const config = {
      class:'modal-dialog-centered modal-md',
      initialState: {
        title: 'get Date Range Input for Statement of Account'
      }
    };
    
    this.bsModalRef = this.modalService.show(DateInputRangeModalComponent, config);

    this.bsModalRef.content.returnDateRangeEvent.subscribe((dateRange: any) => {
      if(dateRange.fromDate.getFullYear < 2000 || dateRange.uptoDate.getFullYear < 2000) {
        this.toastr.warning('failed to get the date range');
        return;
      }
      console.log(dateRange);
      let route = '/finance/statementofaccount/' + accountid + '/' + dateRange.fromDate.toISOString() + '/' + dateRange.uptoDate.toISOString();
      console.log(route);

      this.router.navigate(
          [route], 
          { state: 
            { 
              userobject: this.user,
              dateRange: dateRange,
              returnUrl: '/finance/coalist' 
            } }
        );
    }, (error: any) => {
      console.log('error in invoking Date Range input modal', error);
    })



  }
  
  deleteCoa(accountid: number) {

    if(this.confirmService
      .confirm('Do you want to delete the Chart of Account?  You may want to rename it, instead of deleting it!', 'confirm Delete Chart of account')
      .subscribe(response => {
        if(!response ) {
          this.toastr.info('deletion request canceled');
          return;
        }
      })
      )

    this.service.deleteCOA(accountid).subscribe(response  => {
      if (response) {
        this.removeFromCache(accountid);
        
      } else {
        this.toastr.error('failed to delete the Chart of Account');
      }
    }, error => {
      this.toastr.error('failed to delete the Chart of Account', error);
    })
 
  }

  removeFromCache(id: number) {
    this.service.deleteFromCache(id);
    var index=this.coas.findIndex(x => x.id==id);
    this.coas.splice(index,1);
    this.toastr.success('Chart of account deleted');
  }

  onSortSelected(sort: any) {
    if(this.sParams.sort===sort) return;

    this.sParams.sort = sort;
    this.service.setParams(this.sParams);
    this.getCOAs();
  }

  
  returnToCaller() {
    console.log('return to caller in coalist:', this.returnUrl);
    this.router.navigateByUrl(this.returnUrl || '' );
  }

}
