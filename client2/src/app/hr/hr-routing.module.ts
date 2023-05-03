import { NgModule } from '@angular/core';
import { CvAssessComponent } from './cv-assess/cv-assess.component';
import { RouterModule } from '@angular/router';
import { HrIndexComponent } from './hr-index/hr-index.component';
import { AssessmentStddComponent } from './assessment-stdd/assessment-stdd.component';
import { AssessmentStddQsResolver } from '../resolvers/assessmentStddQsResolver';
import { AssessmentStddQResolver } from '../resolvers/assessmentStddQResolver';
import { HrchecklistComponent } from './hrchecklist/hrchecklist.component';
import { OpenOrderItemCategoriesResolver } from '../resolvers/openOrderItemCategoriesResolver';
import { AssessmentQbankComponent } from './assessment-qbank/assessment-qbank.component';
import { StddqEditComponent } from './stddq-edit/stddq-edit.component';
import { AssessmentQBankResolver } from '../resolvers/assessmentQBankResolver';
import { CategoryListResolver } from '../resolvers/categoryListResolver';
import { AssessComponent } from './assess/assess.component';
import { OrderResolver } from '../resolvers/orderResolver';
import { AssessmentQsResolver } from '../resolvers/assessmentQsResolver';
import { AssessQComponent } from './assess-q/assess-q.component';
import { OrderItemBriefResolver } from '../resolvers/orderItemBriefResolver';
import { OpenRequirementsResolver } from '../resolvers/open-requirements.resolver';
import { CandidateAssessedResolver } from '../resolvers/candidate-assessed.resolver';
import { CommonModule } from '@angular/common';


const routes = [
  {path: '', component: HrIndexComponent,  data: {breadcrumb: 'HR Division'}},
  {path: 'stddqs', component: AssessmentStddComponent,
    resolve: { stddqs: AssessmentStddQsResolver },data: {breadcrumb: {breadcrumb: 'Standard Assessment Questions'}}
  },
  
  {path: 'editstdd/:id', component: StddqEditComponent,
    resolve: {stddq: AssessmentStddQResolver}
  },

  {path: 'checklist', component: HrchecklistComponent,
    resolve: {
      //stddqs: AssessmentStddQsResolver,
      openorderitems: OpenOrderItemCategoriesResolver
    }
  },

  {path: 'qs', component: AssessmentQbankComponent,
    resolve: {
      qs: AssessmentQBankResolver,
      categories: CategoryListResolver
  }},
  
  {path: 'orderassess/:id', component: AssessComponent,
    resolve: {
      order: OrderResolver
    }
  },
  
  {path: 'itemassess/:id', component: AssessQComponent,
    resolve: {
      assessment: AssessmentQsResolver,
      itembrief: OrderItemBriefResolver
    }}, 
  
  {path: 'cvassess/:id', component: CvAssessComponent,
    //canDeactivate: [PreventUnsavedChangesGuard],
    resolve: {
      openOrderItemsBrief: OpenRequirementsResolver,
      assessmentsDto: CandidateAssessedResolver}
  },
  /*
  {path: 'assessments', component: AssessmentsComponent,
    //canDeactivate: [PreventUnsavedChangesGuard],
    resolve: {
      openOrderItemsBrief: OpenOrderItemsResolver}
  },

  { path: 'cvforward', component: CvRefComponent,
    resolve: {
      professions: CategoryListResolver,
      agents: AgentsResolver
    }

  }
  
  /*
  ,


  {path: 'interviews', component: InterviewlistComponent,
  //canDeactivate: [PreventUnsavedChangesGuard],
  resolve: {interviews: InterviewsBriefResolver}
  },
  
  {path: 'interviewindex', component: InterviewIndexComponent}
  */
]


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})

export class HrRoutingModule { }
