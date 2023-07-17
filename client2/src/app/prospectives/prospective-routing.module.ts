import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProspectiveSummaryComponent } from './prospective-summary/prospective-summary.component';
import { CandidateHistoryComponent } from '../candidates/candidate-history/candidate-history.component';
import { ProspectiveSummaryResolver } from '../resolvers/prospectiveSummaryResolver';
import { ProspectiveListComponent } from './prospective-list/prospective-list.component';
import { ContactResultsResolver } from '../resolvers/contactResultsResolver';
import { PreventUnsavedChangesForprospectsGuard } from '../guards/prevent-unsaved-changes-prospects.guard';
import { HrGuard } from '../guards/hr.guard';
import { EmployeeIdsAndKnownAsResolver } from '../resolvers/employeeIdsAndKnownAsResolver';
import { RouterModule } from '@angular/router';

const routes = [
  {path: '', component: ProspectiveSummaryComponent,   // ProspectiveListingComponent,  // UserHistoryHeaderComponent, 
      resolve: { 
        summary: ProspectiveSummaryResolver,
    },
      data: {breadcrumb: {alias: 'Prospective Summary'}}},  
  
  {path: 'list/details', component: ProspectiveListComponent,   // ProspectiveListingComponent,  // UserHistoryHeaderComponent, 
      resolve: { 
        //prospectives: ProspectiveCandidatesAll,
        results: ContactResultsResolver,
        //employeees: EmployeeIdsAndKnownAsResolver,
    },
      canDeactivate: [PreventUnsavedChangesForprospectsGuard],
      data: {breadcrumb: {alias: 'Prospective Candidates by CategoryRef'}}},  
  
  {path: 'prospectivelist/:categoryRef/:dated/:status', component: ProspectiveListComponent,
          resolve: { 
            //prospectives: ProspectiveCandidatesByCategoryRefResolver,
            results: ContactResultsResolver,
            //employeees: EmployeeIdsAndKnownAsResolver,
        },
        canDeactivate: [PreventUnsavedChangesForprospectsGuard],
        data: {breadcrumb: {alias: 'Prospective Candidates by CategoryRef'}}},  

  {path: 'list/:id', component: ProspectiveListComponent, canActivate: [HrGuard],
    resolve: 
      { //prospectivePaging: ProspectiveCandidatesResolver,
        results: ContactResultsResolver,
        employees: EmployeeIdsAndKnownAsResolver},
    data: {breadcrumb: {alias: 'Prospective candidates'}} },
  
  /* {path: '/:id', component: ProspectiveListingComponent, 
    resolve: 
      { 
        prospectiveListing: ProspectiveListingResolver,
        results: ContactResultsResolver,
        employees: EmployeeIdsAndKnownAsResolver},
    data: {breadcrumb: {alias: 'Prospective candidates'}} },
  */
  {path: 'historyfromprospective/:prospectiveId', component: CandidateHistoryComponent, 
      data: {breadcrumb: {alias: 'Prospective Candidate History'}}},
  
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ]
})

export class ProspectiveRoutingModule { }
