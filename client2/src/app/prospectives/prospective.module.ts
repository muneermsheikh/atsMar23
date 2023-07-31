import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProspectiveItemComponent } from './prospective-item/prospective-item.component';
import { ProspectiveListComponent } from './prospective-list/prospective-list.component';
import { ProspectiveSummaryComponent } from './prospective-summary/prospective-summary.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ProspectiveRoutingModule } from './prospective-routing.module';



@NgModule({
  declarations: [
    ProspectiveItemComponent,
    ProspectiveListComponent,
    ProspectiveSummaryComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ProspectiveRoutingModule
  ]
})
export class ProspectiveModule { }
