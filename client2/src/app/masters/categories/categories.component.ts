import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { paramsMasters } from 'src/app/shared/params/masters/paramsMasters';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { MastersService } from '../masters.service';
import { CategoryEditModalComponent } from '../category-edit-modal/category-edit-modal.component';
import { IUser } from 'src/app/shared/models/admin/user';
import { Navigation, Router } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';

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
  returnUrl='';

  constructor(
    private mastersService: MastersService,
      private modalService: BsModalService,
      private toastr: ToastrService
      , private router: Router
      , private bcService: BreadcrumbService
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
    var id: number, categoryString: string;
    id=category.id;
    categoryString=category.name;

    const initialState = {
      str: categoryString
    };
    this.bsModalRef = this.modalService.show(CategoryEditModalComponent, {initialState});

    //returned from modal
    this.bsModalRef.content.update.updateStringName.subscribe((values: any) => {
      if(values === categoryString) {
        this.toastr.warning('category value not changed');
        return;
      } else {
        this.mastersService.updateCategory(id, values).subscribe(response => {
          this.toastr.success('category value updated');
        }, error => {
          this.toastr.error(error);
        })
      }
    })
  }

  deleteCategory(id: number){
    
  }

}
