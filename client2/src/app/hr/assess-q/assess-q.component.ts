import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IOrderItemBriefDto } from 'src/app/shared/dtos/admin/orderItemBriefDto';
import { IAssessment } from 'src/app/shared/models/admin/assessment';
import { StddqsService } from '../stddqs.service';
import { AssessmentService } from '../assessment.service';
import { IAssessmentQ } from 'src/app/shared/models/admin/assessmentQ';
import { AccountService } from 'src/app/account/account.service';
import { take } from 'rxjs/operators';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-assess-q',
  templateUrl: './assess-q.component.html',
  styleUrls: ['./assess-q.component.css']
})
export class AssessQComponent implements OnInit {

  orderitem?: IOrderItemBriefDto;
  assess?: IAssessment;
  user?: IUser;
  totalMarks=0;
  returnUrl: string = '/orders'
  form: FormGroup=new FormGroup({});
  orderitemid: number=0;

  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router, private accountsService: AccountService,
    private stddqservice: StddqsService,
    private assessService: AssessmentService, 
    private toastr: ToastrService,
    private fb: FormBuilder) {
        this.orderitemid = this.activatedRoute.snapshot.params['id'];
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

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
              if(nav.extras.state.object) {
                this.orderitem=nav.extras.state.object;
                console.log('via navigation: orderitem:', this.orderitem);
              }
          }
          //this.bcService.set('@editOrder',' ');
     }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      //this.orderitem = data.itembrief;
      this.assess= data.assessment;

      this.createForm();
      if (this.assess) {
        this.patchForm(this.assess);
        this.calculateTotals();
      } 
    })
  }

  createForm() {
    this.form = this.fb.group({
      id: 0, orderAssessmentId: 0, orderItemId: 0,
      orderId: 0, orderNo: 0, categoryId: 0, categoryName: '',
      orderItemAssessmentQs: this.fb.array([])
     })
  }

  patchForm(p:IAssessment) {
    this.form.patchValue({
      id: p.id, orderAssessmentId: p.orderAssessmentId, 
      orderItemId: p.orderItemId, orderId: p.orderId, 
      orderNo: p.orderNo, categoryId: p.categoryId, 
      categoryName: p.categoryName
    })
    if(p.orderItemAssessmentQs) this.form.setControl('orderItemAssessmentQs', this.setExistingItems(p.orderItemAssessmentQs));
  }

  setExistingItems(items: IAssessmentQ[]): FormArray {
      const formArray = new FormArray([]);
    items.forEach(i => {
      formArray.push(this.fb.group({
        id: i.id, assessmentId: i.assessmentId, orderItemId: i.orderItemId,
        orderId: i.orderId, questionNo: i.questionNo, subject: i.subject,
        question: i.question, maxPoints: i.maxPoints, isMandatory: i.isMandatory
      }))
    });

    return formArray;
  }

  addAssessmentQuestionsForCategory() {
      console.log('orderitem Id i assess-q.component.ts addAssessmentQForategory', this.orderitemid);

        this.assessService.getAssessmentQBankOfCategoryId(this.orderitemid, this.orderitem!.categoryId).subscribe({
          next: (stddQs: IAssessmentQ[]) => {
              if(stddQs == null) {
                this.toastr.warning('failed to retrieve Assessment Questions for the category.  Make sure the category has defined Assessment Questions');
              } else {
                this.form.setControl('orderItemAssessmentQs', this.setExistingItems(stddQs));
                this.calculateTotals();
              }
            },
          error: error => this.toastr.error('Error in getting standard Questions from API,', error  )
      });
      
  }

  addStddAssessmentQuestions() {
      this.stddqservice.getStddQs().subscribe({
        next: (stddQs: IAssessmentQ[]) => {
          if(stddQs == null) {
            this.toastr.warning('failed to retrieve Standard Questions');
          } else {
            var itemid = this.form.get('orderItemId')!.value;
            var orderid = this.form.get('orderId')!.value;
            var assessmentid = this.form.get('id')?.value;
              stddQs.forEach(s => {
                s.orderItemId=itemid;
                s.orderId=orderid;
                s.isMandatory=s.isMandatory===null || s.isMandatory===undefined ? false : s.isMandatory;
                s.assessmentId=assessmentid;
              })
            this.form.setControl('orderItemAssessmentQs', this.setExistingItems(stddQs));
            this.calculateTotals();

          }
        },
        error: error => this.toastr.error('Error in getting standard Questions from API,', error  )
      });
  
  }

  get orderItemAssessmentQs(): FormArray {
    return this.form.get('orderItemAssessmentQs') as FormArray;
  }

  newOrderItemAssessmentQ(): FormGroup{
    var qno = this.orderItemAssessmentQs.length+1;
    return this.fb.group({
      id: 0, assessmentId: 0, orderItemId: 0,
      orderId: 0, questionNo: qno, question: '',
      subject: '', maxMarks: 0, isMandatory: false
    })
  }

  addOrderItemAssessmentQ(){
    this.orderItemAssessmentQs.push(this.newOrderItemAssessmentQ());
  }

  removeOrderItemAssessmentQ(i: number) {
    this.orderItemAssessmentQs.removeAt(i);
    this.orderItemAssessmentQs.markAsDirty();
    this.orderItemAssessmentQs.markAsTouched();
    this.calculateTotals();
  }

  update() {
    this.assessService.updateAssessment(this.form.value).subscribe(response => {
      if (response) {
        this.toastr.success('updated the Assessment Question');
        
      } else {
        this.toastr.warning('failed to update the Assessment Question');
      }
    }, error => {
      this.toastr.error('error updating the assessment question', error);
    })
  }

  calculateTotals() {
    //const subtotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    //var tot = this.assess.orderItemAssessmentQs.map(x => x.maxMarks).reduce((a, b) => a + b);
    this.totalMarks =  this.orderItemAssessmentQs.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);

  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }
 
}
