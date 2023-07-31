import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IAssessmentStandardQ } from 'src/app/shared/models/admin/assessmentStandardQ';
import { IUser } from 'src/app/shared/models/admin/user';
import { StddqsService } from '../stddqs.service';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-stddq-edit',
  templateUrl: './stddq-edit.component.html',
  styleUrls: ['./stddq-edit.component.css']
})
export class StddqEditComponent implements OnInit {

  stddq: IAssessmentStandardQ|undefined;
  form: FormGroup=new FormGroup({});
  user?: IUser;

  isAddMode: boolean=false;

  constructor(private activatedRoute: ActivatedRoute, 
    private service: StddqsService, private fb: FormBuilder,
    private toastr: ToastrService, private router: Router,
    private confirmService: ConfirmService) { }

  ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => { 
        this.stddq = data.stddq;
        this.isAddMode = this.activatedRoute.snapshot.params['id']==='';
        console.log('stddQ Edit ngONInit', this.stddq);
        console.log('isAddMode=', this.isAddMode);
    })

    this.createForm();

  }


  createForm() {
    this.form = this.fb.group({

      id: 0, assessmentParameter: ['', [Validators.required, Validators.maxLength(150)]],
      qNo: [0, Validators.required],
      question: ['', [Validators.required, Validators.maxLength(200)]],
      maxPoints: [0, [Validators.min(1), Validators.max(100)]]
    })

    if (this.stddq) {
        if (!this.isAddMode) {
          this.form.patchValue({
            id: this.stddq.id,
            assessmentParameter: this.stddq.subject,
            qNo: this.stddq.questionNo,
            question: this.stddq.question,
            maxPoints: this.stddq.maxPoints
          })
        }
    }
  }

  onSubmit() {
    if(this.isAddMode) {
      this.createQ();
    } else {
      this.updateQ();
    }
  }

  private createQ() {
    this.service.createStddQ(this.form.value).subscribe(response => {
      this.toastr.success('Standard Assessment Question created');
      this.router.navigateByUrl('./hr/stddqs');
    }, error => {
      this.toastr.warning('failed to create the standard Q', error);
    })
  }

  private updateQ() {
    const q = this.form.value;
    let qs: IAssessmentStandardQ[]=[];
    qs.push(q);
    this.service.updateStddQ(qs).subscribe(response => {
      if (response) {
        this.toastr.success('stdd Q updated');
        this.router.navigateByUrl('./hr/stddqs');
      } else {
        this.toastr.warning('failed to update the standard Question');
      }
    }, error => {
      this.toastr.error('Error ', error);
    })
  }

  onClose() {
    if (this.form.dirty) {
      this.confirmService.confirm('This form has data that is not saved', 
        'do you want to close the form without savign the data?', 'Yes, Close', 'no').subscribe(
          response => {
            if (response) {
              this.router.navigateByUrl('/hr/stddqs');
            } else {
              this.toastr.info('close aborted');
            }
          }
        )
    } else {
      this.router.navigateByUrl('/hr/stddqs');
    }
  }

}
