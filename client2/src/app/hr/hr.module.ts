import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CvAssessComponent } from './cv-assess/cv-assess.component';
import { HrIndexComponent } from './hr-index/hr-index.component';
import { HrChecklistComponent } from './hr-checklist/hr-checklist.component';
import { AssessQComponent } from './assess-q/assess-q.component';
import { AssessmentStddComponent } from './assessment-stdd/assessment-stdd.component';
import { AssessmentQbankComponent } from './assessment-qbank/assessment-qbank.component';
import { AssessmentLineComponent } from './assessment-line/assessment-line.component';
import { AssessComponent } from './assess/assess.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HrRoutingModule } from './hr-routing.module';
import { SharedModule } from '../shared/shared.module';
import { HrchecklistComponent } from './hrchecklist/hrchecklist.component';
import { StddqEditComponent } from './stddq-edit/stddq-edit.component';
import { CandidateAssessmentComponent } from './candidate-assessment/candidate-assessment.component';
import { EditAssessmentQStddModalComponent } from './edit-assessment-q-stdd-modal/edit-assessment-q-stdd-modal.component';



@NgModule({
  declarations: [
    CvAssessComponent,
    HrIndexComponent,
    HrChecklistComponent,
    AssessQComponent,
    AssessmentStddComponent,
    AssessmentQbankComponent,
    AssessmentLineComponent,
    AssessComponent,
    StddqEditComponent,
    HrchecklistComponent,
    CandidateAssessmentComponent,
    EditAssessmentQStddModalComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    HrRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class HrModule { }
