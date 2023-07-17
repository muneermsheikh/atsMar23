import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { paramsMasters } from 'src/app/shared/params/masters/paramsMasters';
import { MastersService } from '../masters.service';
import { ToastrService } from 'ngx-toastr';
import { Navigation, Router } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { IUser } from 'src/app/shared/models/admin/user';
import { MasterEditModalComponent } from '../master-edit-modal/master-edit-modal.component';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { IQualification } from 'src/app/shared/models/hr/qualification';

@Component({
  selector: 'app-qualifications',
  templateUrl: './qualifications.component.html',
  styleUrls: ['./qualifications.component.css']
})
export class QualificationsComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  qualifications: IQualification[]=[];
  cParams= new paramsMasters();
  totalCount: number=0;
  bsModalRef: BsModalRef|undefined;
  returnUrl = '/admin';
  user?: IUser;

  constructor(
    private mastersService: MastersService,
      private modalService: BsModalService,
      private toastr: ToastrService
      , private router: Router
      , private bcService: BreadcrumbService
      , private confirmService: ConfirmService
  ) { 
          //this.routeId = this.activatedRoute.snapshot.params['id'];
      //this.router.routeReuseStrategy.shouldReuseRoute = () => false;

      //this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

      //navigationExtras
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          //this.bolNavigationExtras=true;
          if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;
              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
          this.bcService.set('@Qualifications',' ');

  }

  ngOnInit(): void {
    this.mastersService.setParams(this.cParams);
    this.getQualifications(false);
  }

  getQualifications(useCache=false) {
    //gets paginated categories

    this.mastersService.getQualifications(useCache).subscribe((response:any) => {
      this.qualifications = response.data;
      this.totalCount = response?.count!;
    }, error => {
      console.log(error);
    })
  }

  onSearch() {
    const params = this.mastersService.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.mastersService.setParams(params);
    this.getQualifications();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cParams = new paramsMasters();
    this.mastersService.setParams(this.cParams);
    this.getQualifications();
  }

  onPageChanged(event: any){
    const params = this.mastersService.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.mastersService.setParams(params);
      this.getQualifications(true);
    }
  }

  editQualification(index: number, categoryString: string) {
    const initialState = {
      title: 'Edit Qualification',
      caption: 'Qualification ',
      returnString: categoryString
    };
    this.bsModalRef = this.modalService.show(MasterEditModalComponent, {initialState});

    //returned from modal
    this.bsModalRef.content.editedStringName.subscribe((values: any) => {
      if(values === categoryString) {
        this.toastr.warning('Qualification value not changed');
        return;
      } else {
        this.mastersService.updateQualification(index, values).subscribe(response => {
          if(response) {
            this.toastr.success('Qualification value updated');
          } else {
            this.toastr.warning('failed to update the qualification');
          }
        }, (error: any) => {
          this.toastr.error(error);
        })
      }
    })
  }

  deleteQ(id: number, cat: string){

    this.confirmService.confirm("Confirm", "Confirm if you want to delete the Qualification '" + 
      cat + "' ").subscribe({
        next: response => {
          if(!response) return;
        },
        error: err => {
          this.toastr.error('Error occured in getting the confirmation');
          return;
        }
      })
    
    this.mastersService.deleteQualification(id).subscribe({
      next: response => {
        if(response)  {
          this.toastr.success('Qualification deleted')
         } else {
          this.toastr.info('failed to delete the Qualification')
         }
      }, 
        error: err => this.toastr.error('error in deleting the Qualification')
    })
  }

  goBack() {
    this.router.navigateByUrl(this.returnUrl);
  }

}
