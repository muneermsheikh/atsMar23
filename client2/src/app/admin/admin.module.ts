import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { AdminIndexComponent } from './admin-index/admin-index.component';
import { DlForwardComponent } from './dl-forward/dl-forward.component';
import { CategoryRefCodePipe } from './category-ref-code.pipe';
import { SelectionComponent } from './selection/selection.component';
import { MessagesComponent } from './messages/messages.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { EmploymentLineComponent } from './employment-line/employment-line.component';
import { EmploymentComponent } from './employment/employment.component';
import { EmploymentModalComponent } from './employment-modal/employment-modal.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CvrefComponent } from './cvref/cvref.component';
import { CvrefLineComponent } from './cvref-line/cvref-line.component';
import { AdminRoutingModule } from './admin-routing.module';



@NgModule({
  declarations: [
    AdminIndexComponent,
    DlForwardComponent,
    CategoryRefCodePipe,
    SelectionComponent,
    MessagesComponent,
    EmploymentLineComponent,
    EmploymentComponent,
    EmploymentModalComponent,
    CvrefComponent,
    CvrefLineComponent,
    
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AngularEditorModule,
    RouterModule,
    SharedModule,
    AdminRoutingModule
  ]
})
export class AdminModule { }
