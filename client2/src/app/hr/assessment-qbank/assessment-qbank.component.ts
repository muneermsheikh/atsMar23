import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AssessmentQBank, IAssessmentQBank, IAssessmentQBankItem } from 'src/app/shared/models/admin/assessmentQBank';
import { IUser } from 'src/app/shared/models/admin/user';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { qBankParams } from 'src/app/shared/params/admin/qBankParams';
import { BreadcrumbService } from 'xng-breadcrumb';
import { QbankService } from '../qbank.service';

@Component({
  selector: 'app-assessment-qbank',
  templateUrl: './assessment-qbank.component.html',
  styleUrls: ['./assessment-qbank.component.css']
})
export class AssessmentQbankComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  qs: IAssessmentQBank[]=[];
  q: IAssessmentQBank|undefined;
  categories: IProfession[]=[];
  user?: IUser;
  
  qBankParams = new qBankParams();
  totalCount: number=0;
  assessmentParameters: string[]=[];
  existingQBankCats: IProfession[]=[];
  selectedCategoryId?: IProfession
  lastCategoryId: number=0;

  form: FormGroup=new FormGroup({});
  
  constructor(private service: QbankService, private bcService: BreadcrumbService,
    private fb: FormBuilder, private activatedRoute: ActivatedRoute,
    private toastr: ToastrService) { 
      this.bcService.set('@assessmentQBank', ' ');
    }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.qs = data.qs,
      this.categories = data.categories
    })

    this.createForm();  
  }

  createForm() {
    this.form = this.fb.group({
      id: [null],
      categoryId: 0,
      categoryName: '',
      assessmentQBankItems: this.fb.array([])
    } );
  }

  createCategoriesForm() {
    this.form = this.fb.group({
      categories: this.fb.array([])
    })
  }

  createNewQ() {
    console.log('increatenewQ', this.q);
    if (this.q === null || this.q === undefined && this.selectedCategoryId === null) {
      this.toastr.info('Please select a category and then invoke this function');
      return;
    }
    if (this.selectedCategoryId == null) {
      this.toastr.error('please select a category from the dropdown box');
      return null;
    } 

    var newQ: IAssessmentQBank | undefined; // = new AssessmentQBank();

    newQ!.categoryId=this.selectedCategoryId.id;
    newQ!.categoryName=this.selectedCategoryId.name;
    this.service.insert(newQ!).subscribe((response: any) => {
        this.q = response;
        this.editQBank(this.q!);
    }, (error: any) => {
      this.toastr.error('failed to insert the new Assessment Question', error);
    })

    return null;
    
  }

  showQ() {
    
    if (this.selectedCategoryId === null) 
    {
      this.toastr.info('selected category not set');
      this.editQBank(undefined);
      return;
    }

    if (this.selectedCategoryId!.id === this.lastCategoryId) return;

    this.service.getQ(this.selectedCategoryId!.id).subscribe((response: any) => {
      this.q = response;
      console.log(this.q);
      this.editQBank(this.q!);
    })
    this.lastCategoryId = this.selectedCategoryId!.id;
  }

  update(){
    this.service.update(this.form.value).subscribe((response:any) => {
      if (response) {
        this.toastr.success('updated the Assessment Question');
      } else {
        this.toastr.warning('failed to update the assessment Question');
      }
  }, (error: any) => {
    this.toastr.error('failed to udpate', error);
  })
}

  assessmentQBankItems() : FormArray {
    return this.form.get('assessmentQBankItems') as FormArray;
  }


  clearItemsArray() {
    const control = <FormArray>this.form.controls['assessmentQBankItems'];
    for(let i = control.length-1; i >= 0; i--) {
      control.removeAt(i)
  }

  }
  editQBank(qb: IAssessmentQBank|undefined) {
    console.log('in editQBank, qb is: ', qb);
    if(qb==null) {
      this.clearItemsArray();
      return;
    }
    this.form.patchValue( {
      id: qb.id,
      categoryId: qb.categoryId,
      categoryName: qb.categoryName
    });

    if (qb.assessmentQBankItems !== null) {
        this.form.setControl('assessmentQBankItems', this.setExistingItems(qb.assessmentQBankItems));
    }
  }

  setExistingItems(items: IAssessmentQBankItem[]): FormArray {
    const formArray = new FormArray([]);

    items.forEach(item => {
      formArray.push(this.fb.group({
        id: item.id,
        assessmentQBankId: item.assessmentQBankId,
        assessmentParameter: item.assessmentParameter,
        qNo: item.qNo,
        question: item.question,
        maxPoints: item.maxPoints
      }));
    })
    return formArray;
  }

  newAssessmentQBankItem(): FormGroup {
    return this.fb.group({
      id: 0,
      assessmentQBankId: 0,
      assessmentParameter: ['', Validators.required],
      qNo: [0, Validators.required],
      isStandardQ: [false],
      question: ['', Validators.required],
      maxPoints: [0, Validators.required]
    })
  }

  addAssessmentQBankItem() {
    this.assessmentQBankItems().push(this.newAssessmentQBankItem());
  }

  removeAssessmentQBankItem(qItemIndex: number) {
    this.assessmentQBankItems().removeAt(qItemIndex);
    this.assessmentQBankItems().markAsDirty();
    this.assessmentQBankItems().markAsTouched();
  }

  loadQBank(){
    this.form.patchValue(this.qs);
    this.qs.forEach(q => {
      if(q.assessmentQBankItems !== null) {
          for(const item of q.assessmentQBankItems) {
            this.assessmentQBankItems().push(new FormControl(q));
          }
      }
    })
  }

  onSubmit() {
    console.log(this.form.value);
  }

  onCategoryChange(category: string) {
    console.log(category);
  }

  /*
  onPageChanged(event: any){
    const params = this.service.getQParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setQParams(params);
      this.getCVs(true);
    }
  }
  */
 
}
