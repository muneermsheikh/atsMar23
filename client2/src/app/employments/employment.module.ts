import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmploymentLineComponent } from './employment-line/employment-line.component';
import { EmploymentListComponent } from './employment-list/employment-list.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmploymentIndexComponent } from './employment-index/employment-index.component';
import { EmploymentRoutingModule } from './employment-routing.module';



@NgModule({
  declarations: [
    EmploymentLineComponent,
    EmploymentListComponent,
    EmploymentIndexComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    EmploymentRoutingModule
  ]
})
export class EmploymentModule { }
