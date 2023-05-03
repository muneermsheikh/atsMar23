import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidatesListingComponent } from './candidates-listing/candidates-listing.component';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { CandidateItemComponent } from './candidate-item/candidate-item.component';
import { ProspectiveListComponent } from './prospective-list/prospective-list.component';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { CandidatesRoutingModule } from './candidates-routing.module';
import { CustomerNamePipe } from '../shared/pipes/customerNamePipe';
import { ChecklistModalComponent } from './checklist-modal/checklist-modal.component';



@NgModule({
  declarations: [
    CandidatesListingComponent,
    CandidateEditComponent,
    CandidateItemComponent,
    ProspectiveListComponent,
    CandidateHistoryComponent,
    CustomerNamePipe,
    ChecklistModalComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule,
    CandidatesRoutingModule
  ]
})
export class CandidatesModule { }
