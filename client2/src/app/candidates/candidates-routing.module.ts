import { NgModule } from '@angular/core';
import { CandidatesListingComponent } from './candidates-listing/candidates-listing.component';
import { CategoryListResolver } from '../resolvers/categoryListResolver';
import { QualificationListResolver } from '../resolvers/qualificationListResolver';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { CandidateResolver } from '../resolvers/candidateResolver';
import { UploadComponent } from './upload/upload.component';
import { PreventUnsavedChangesGuard } from '../guards/prevent-unsaved-changes.guard';
import { HrGuard } from '../guards/hr.guard';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';
import { CandidateHistoryResolver } from '../resolvers/candidateHistoryResolver';
import { EmployeeIdsAndKnownAsResolver } from '../resolvers/employeeIdsAndKnownAsResolver';
import { ContactResultsResolver } from '../resolvers/contactResultsResolver';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AgentsResolver } from '../resolvers/agents.resolver';
import { RegisterComponent } from '../account/register/register.component';
import { PreventUnsavedChangesRegisterGuard } from '../guards/prevent-unsaved-changes-register.guard';

const routes = [
  {path: '', component: CandidatesListingComponent,
    resolvers: {
      //candidateBriefs: CandidateBriefResolver,
      professions: CategoryListResolver,
      qualifications: QualificationListResolver
    }},
  
    {path: 'cached', component: CandidatesListingComponent,
    resolvers: {
      //candidateBriefs: CandidateBriefResolver,
      professions: CategoryListResolver,
      qualifications: QualificationListResolver
    }},

  {path: 'add', component:RegisterComponent , data: {breadcrumb: {alias: 'register'}},
    resolve: {
      //employees: EmployeeIdsAndKnownAsResolver,
      professions: CategoryListResolver,
      agents: AgentsResolver,

    },
    canDeactivate: [PreventUnsavedChangesRegisterGuard],
  },
  {path: 'edit/:id', component: RegisterComponent,  // CandidateEditComponent, 
    /*resolve: {
      categories: CategoryListResolver,
      qualifications: QualificationListResolver,
      candidate: CandidateResolver
    }, */
      canActivate: [HrGuard],
      data: {breadcrumb: {alias: 'candidate Edit/Add'}},
      canDeactivate: [PreventUnsavedChangesGuard],
    },
  {path: 'view/:id', component: CandidateEditComponent , data: {breadcrumb: {alias: 'candidateView'}}},
  {path: 'upload/:id', component: UploadComponent, canActivate: [HrGuard], data: {breadcrumb: {alias: 'upload'}}},
  {path: 'download/:id', component: UploadComponent, canActivate: [HrGuard], data: {breadcrumb: {alias: 'upload'}}},
  {path: 'newtask/:id', component: UploadComponent, canActivate: [HrGuard], data: {breadcrumb: {alias: 'upload'}}},
  
    //resolve: { historyHeaders: UserHistoryHeaderResolver},
      //data: {breadcrumb: {alias: 'User History Headers'}}
  {path: 'historyfromcvid/:id', component: CandidateHistoryComponent, canActivate: [HrGuard],
    resolve: 
      { 
        history: CandidateHistoryResolver,
        results: ContactResultsResolver,
        employees: EmployeeIdsAndKnownAsResolver},
      data: {breadcrumb: {alias: 'CandidateHistoryFromCVId'}}},
  {path: 'historyfromProspectiveId/:id', component: CandidateHistoryComponent, canActivate: [HrGuard],
      resolve: 
        { 
          history: CandidateHistoryResolver,
          results: ContactResultsResolver,
          employees: EmployeeIdsAndKnownAsResolver},
        data: {breadcrumb: {alias: 'CandidateHistoryFromCVId'}}},
  {path: 'historybyid/:id', component: CandidateHistoryComponent, 
      resolve: 
        { history: CandidateHistoryResolver, // CandidateHistoryFromHistoryIdResolver,
          results: ContactResultsResolver,
          employees: EmployeeIdsAndKnownAsResolver},
        data: {breadcrumb: {alias: 'customerDetail'}}},

  {path: 'history/:id', component: CandidateHistoryComponent, 
    resolve: 
      { 
        history: CandidateHistoryResolver,  // CandidateHistoryFromHistoryIdResolver,
        results: ContactResultsResolver,
        employees: EmployeeIdsAndKnownAsResolver},
      data: {breadcrumb: {alias: 'CandidateHistory'}}},
  {path: 'historybyid/:id', component: CandidateHistoryComponent, 
      resolve: 
        { history: CandidateHistoryResolver,  // CandidateHistoryFromHistoryIdResolver,
          results: ContactResultsResolver,
          employees: EmployeeIdsAndKnownAsResolver},
        data: {breadcrumb: {alias: 'customerDetail'}}}
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
export class CandidatesRoutingModule { }
