import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { paramsMasters } from 'src/app/shared/params/masters/paramsMasters';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { MastersService } from '../masters.service';
import { IUser } from 'src/app/shared/models/admin/user';
import { Navigation, Router } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { MasterEditModalComponent } from '../master-edit-modal/master-edit-modal.component';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  cats: IProfession[]=[];
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
          //this.bolNavigationExtras=true;
              if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
          this.bcService.set('@Categories',' ');
   }

  ngOnInit(): void {
    this.mastersService.setParams(this.cParams);
    this.getCats(false);
  }

  getCats(useCache=false) {
    //gets paginated categories
    this.mastersService.getCategories(useCache).subscribe(response => {
      this.cats = response?.data!;
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
    this.getCats();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cParams = new paramsMasters();
    this.mastersService.setParams(this.cParams);
    this.getCats();
  }

  onPageChanged(event: any){
    const params = this.mastersService.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.mastersService.setParams(params);
      this.getCats(true);
    }
  }

  openCategoryEditModal(category: IProfession) {
 
    var categoryname=category.name;
    const initialState = {
      title: 'Edit Category',
      caption: 'Category ',
      returnString: categoryname,
    };

    this.bsModalRef = this.modalService.show(MasterEditModalComponent, {initialState});

    //returned from modal
    this.bsModalRef.content.editedStringName.subscribe((values: any) => {
      if(values === category.name) {
        this.toastr.warning('category value not changed');
        return;
      } else {
        this.mastersService.updateCategory(category.id, values).subscribe(response => {
          this.toastr.success('category value updated');
        }, error => {
          this.toastr.error(error);
        })
      }
    })
  }

  
  deleteCategory(id: number, cat: string){

    this.confirmService.confirm("Confirm", "Confirm if you want to delete the Category '" + 
      cat + "' ").subscribe({
        next: response => {
          if(!response) return;
        },
        error: err => {
          this.toastr.error('Error occured in getting the confirmation');
          return;
        }
      })
      
    this.mastersService.deleteCategory(id).subscribe({
      next: succeeded => {
        if(succeeded) this.toastr.success("Category deleted");
      },
      error: err => this.toastr.error('Error in deleting the category', err)
    });
    
  }

  goBack() {
    this.router.navigateByUrl(this.returnUrl);
  }
}
