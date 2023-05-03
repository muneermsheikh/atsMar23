import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { paramsMasters } from 'src/app/shared/params/masters/paramsMasters';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { MastersService } from '../masters.service';
import { CategoryEditModalComponent } from '../category-edit-modal/category-edit-modal.component';

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

  constructor(
    private mastersService: MastersService,
      private modalService: BsModalService,
      private toastr: ToastrService
  ) { }

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

  openCategoryEditModal(index: number, categoryString: string) {
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
        this.mastersService.updateCategory(index, values).subscribe(response => {
          this.toastr.success('category value updated');
        }, error => {
          this.toastr.error(error);
        })
      }
    })
  }

  deleteQ(id: number){
    
  }

}
