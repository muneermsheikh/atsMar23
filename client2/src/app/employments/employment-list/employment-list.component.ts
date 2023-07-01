import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

import { employmentParams } from 'src/app/shared/params/admin/employmentParam';
import { EmploymentService } from '../employment.service';
import { AccountService } from 'src/app/account/account.service';
import { IUser } from 'src/app/shared/models/admin/user';
import { catchError, switchMap, take, tap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

import { IEmployment } from 'src/app/shared/models/admin/employment';
import { EmploymentModalComponent } from 'src/app/admin/employment-modal/employment-modal.component';



@Component({
  selector: 'app-employment-list',
  templateUrl: './employment-list.component.html',
  styleUrls: ['./employment-list.component.css']
})
export class EmploymentListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  employments: IEmployment[]=[];
  eParams = new employmentParams();
  totalCount: number=0;
  user?: IUser;

  bsModalRef: BsModalRef | undefined;


  sortOptions = [
    {name:'By Customer', value:'customer'},
    {name:'By Customer Desc', value:'customerdesc'},
    {name:'By Order No', value:'orderno'},
    {name:'By Order No Desc', value:'orderdesc'},
    {name:'By Category', value:'category'},
    {name:'By Category Desc', value:'categorydesc'},
    {name:'By Application No', value:'appno'},
    {name:'By Application No Desc', value:'appnodesc'},
    {name:'By Candidate Name', value:'candidatename'},
    {name:'By Candidate Name Desc', value:'candidatenamedesc'}
  ]

  employmentStatus = [
    {name: 'Approved', value: 'approved'},
    {name: 'Approval Pending', value: 'approvalpending'},
    {name: 'All Status', value: 'allstatus'}
  ]
  constructor(private service: EmploymentService
      , private activatedRoute: ActivatedRoute
      , private accountsService: AccountService
      , private toastr: ToastrService
      , private confirmService: ConfirmService
      , private router: Router
      , private modalService: BsModalService) 
  {
        this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  ngOnInit(): void {
    /*
    this.activatedRoute.data.subscribe(data => {
      this.employments = data.employments;
      this.totalCount = data.count;
    })
    */
    this.getEmployments(false);
    console.log('employments;', this.employments);
   /*
    this.activatedRoute.data.subscribe({
      next: response => {
        console.log('employmentslist response:', response);
        this.employments = response.employments;
        this.totalCount = response.count;
      },
      error: error => console.log(error)
      
    });
    */
  }

  getEmployments(useCache: boolean) {
    this.service.getEmployments(useCache).subscribe({
      next: response => {
        this.employments = response.data;
        this.totalCount = response.count;
      },
      error: error => console.log(error)
    });
  }

  deleteEmployment (id: number){
    this.confirmService.confirm('confirm delete this Employment record', 'confirm delete order').pipe(
      switchMap(confirmed => this.service.deleteEmployment(id).pipe(
        catchError(err => {
          console.log('Error in deleting the order', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted Employment record')),
        //tap(res=>console.log('delete voucher succeeded')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
        () => {
          console.log('deete succeeded');
          this.toastr.success('order deleted');
        },
        (err: any) => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      })
  }

  editEmployment(employment: IEmployment) {

    /*var id= evet;
    let route = '/employments/edit/' + id;
    this.router.navigate([route], { state: { toedit: true, returnUrl: '/employments' } });
  */
    
    const initialState = {
      class: 'modal-dialog-centered modal-lg',
       user: this.user,
       employment
    };
    this.bsModalRef = this.modalService.show(EmploymentModalComponent, {initialState});
    //**TODO** IMPLEMENT SWITCHMAP HERE, TO AVOID SUBSCRIPTION NESTING - CHECK implementation in referral edit */
    //this.bsModalRef.content.updateEmployment.subscribe((values: IEmployment) => {
      this.bsModalRef.content.editEvent.subscribe((values: IEmployment) => {
    this.service.updateEmployment(values).subscribe(() => {
      this.toastr.success("job description updated");
    }, error => {
      this.toastr.error("failed to update the job description");
    })
  })
    
  }

  onSearch() {
    const params = this.service.getEParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setEParams(params);
    this.getEmployments(true);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.eParams = new employmentParams();
    this.service.setEParams(this.eParams);
    this.getEmployments(false);
  }

  onSortSelected(event: any) {
    var sort = event?.target.value;
    this.eParams.pageNumber=1;
    this.eParams.sort = sort;
    this.getEmployments(true);
  }

  onStatusSelected(event: any) {
    const params = this.service.getEParams();
    params.search = event;
    params.pageNumber = 1;
    this.service.setEParams(params);
    this.getEmployments(true);
  }
  
  onPageChanged(event: any){
    const params = this.service.getEParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setEParams(params);
      this.getEmployments(true);
    }
  }

  openEditModal(emp: IEmployment) {

    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        emp : emp,
        title: 'test'
      }
    };

    this.bsModalRef = this.modalService.show(EmploymentModalComponent, config);

    
    this.bsModalRef.content.editEvent.pipe(
      switchMap((values: IEmployment) => this.service.updateEmployment(values).pipe(
        catchError(err => {
          console.log('Error in updating Employment Record', err);
          return of();
        }),
        tap(res => this.toastr.success('Employment Record updated')),
      )),
      catchError(err => {
        console.log(err);
        this.toastr.error('Error in getting updated object from Employment Edit Modal', err);
        return of();
      })
    ).subscribe( () => {
      this.toastr.success('Employment record updated');
      console.log()
    }),
    (err: any) => {
      console.log('unhandled error NOT handled in catch Error(, orif throwError()')
    }
    
   /*
      this.bsModalRef.content.updateEmployment.subscribe((values: IEmployment) => {
        console.log('calling update employment');
          this.service.updateEmployment(values).subscribe(() => {
            this.toastr.success('employment updated');
          }, error => {
            this.toastr.error('failed to update the checklist Service', error);
          })
      })  
    */
  }
}