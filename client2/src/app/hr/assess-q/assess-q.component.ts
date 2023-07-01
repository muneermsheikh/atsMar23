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
  
  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router, private accountsService: AccountService,
    private stddqservice: StddqsService,
    private service: AssessmentService, 
    private toastr: ToastrService,
    private fb: FormBuilder) {
        //this.routeId = this.activatedRoute.snapshot.params['id'];
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
      console.log('cvassess in ngOnInit', this.assess);
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
        question: i.question, maxMarks: i.maxMarks, isMandatory: i.isMandatory
      }))
    });

    return formArray;
  }

  addStddQ() {

    this.stddqservice.getStddQs(true).subscribe(response => {
      const stddqs = response;
      if (stddqs===null) {
        this.toastr.warning('failed to retrieve standard questions');
        return;
      }

      stddqs.forEach(q => {
        this.orderItemAssessmentQs.push(this.fb.group({
          id: q.id, assessmentId: this.assess!.id, orderId: this.assess!.orderId,
          questionNo: q.qNo, subject: q.assessmentParameter, question: q.question, maxMarks: q.maxPoints,
          isMandatory: false
        }))
      })
      this.calculateTotals();
    }, error => {
      this.toastr.error('error - failed to retrieve standard questions');
    })
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
    this.service.updateAssessment(this.form.value).subscribe(response => {
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
    this.totalMarks =  this.orderItemAssessmentQs.value.map((x:any) => x.maxMarks).reduce((a:number, b: number) => a + b,0);

  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }
 
}
