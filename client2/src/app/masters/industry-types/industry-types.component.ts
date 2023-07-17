import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IIndustryType } from 'src/app/shared/models/admin/industryType';
import { IUser } from 'src/app/shared/models/admin/user';
import { paramsMasters } from 'src/app/shared/params/masters/paramsMasters';
import { MastersService } from '../masters.service';
import { ToastrService } from 'ngx-toastr';
import { BreadcrumbService } from 'xng-breadcrumb';
import { Navigation, Router } from '@angular/router';
import { MasterEditModalComponent } from '../master-edit-modal/master-edit-modal.component';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-industry-types',
  templateUrl: './industry-types.component.html',
  styleUrls: ['./industry-types.component.css']
})
export class IndustryTypesComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  indTypes: IIndustryType[]=[];
  cParams= new paramsMasters();
  totalCount: number=0;
  bsModalRef: BsModalRef|undefined;

  user?: IUser;
  returnUrl='/admin';

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
        
              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
          this.bcService.set('@IndustryTypes',' ');
   }

  ngOnInit(): void {
    this.mastersService.setParams(this.cParams);
    this.getIndustryTypes(false);
  }

  getIndustryTypes(useCache=false) {
    //gets paginated categories
    this.mastersService.getIndustryPaged(useCache).subscribe(response => {
      this.indTypes = response?.data!;
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
    this.getIndustryTypes();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cParams = new paramsMasters();
    this.mastersService.setParams(this.cParams);
    this.getIndustryTypes();
  }

  onPageChanged(event: any){
    const params = this.mastersService.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.mastersService.setParams(params);
      this.getIndustryTypes(true);
    }
  }

  openIndTypeEditModal(ind: IIndustryType) {
    
    const initialState = {
      title: 'Edit Industry Type',
      caption: 'Industry:',
      returnString: ind.name
    };

    this.bsModalRef = this.modalService.show(MasterEditModalComponent, {initialState});

    //returned from modal
    this.bsModalRef.content.editedStringName.subscribe((values: any) => {
      if(values === ind.name) {
        this.toastr.warning('Industry Type not changed');
        return;
      } else {
        this.mastersService.updateIndustry(values.id, values).subscribe(response => {
          this.toastr.success('Industry Type updated');
        }, error => {
          this.toastr.error(error);
        })
      }
    })
  }

  deleteIndustry(id: number, ind: string){

    this.confirmService.confirm("Confirm", "Confirm if you want to delete the Industry '" + 
      ind + "' ").subscribe({
        next: response => {
          if(!response) return;
        },
        error: err => {
          this.toastr.error('Error occured in getting the confirmation');
          return;
        }
      })
    
    this.mastersService.deleteIndustry(id).subscribe({
      next: response => {
        if(response)  {
          this.toastr.success('Industry Type deleted')
         } else {
          this.toastr.info('failed to delete the Industry Type')
         }
      }, 
        error: err => this.toastr.error('error in deleting the Industry Type')
    })
  }


  goBack() {
    this.router.navigateByUrl(this.returnUrl);
  }

}
