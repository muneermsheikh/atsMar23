import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallModalComponent } from './call-modal/call-modal.component';
import { CallListComponent } from './call-list/call-list.component';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CallRecordsRoutingModule } from './call-records-routing.module';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [
    CallModalComponent,
    CallListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    CallRecordsRoutingModule,
    SharedModule,
  ]
})
export class CallRecordsModule { }
