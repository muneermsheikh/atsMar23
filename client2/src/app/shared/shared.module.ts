import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagingComponent } from './components/paging/paging.component';
import { PagingHeaderComponent } from './components/paging-header/paging-header.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NavComponent } from './components/nav/nav.component';
import { TextInputComponent } from './components/text-input/text-input.component';
import { DateInputComponent } from './components/date-input/date-input.component';
import { RouterModule } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { NgSelectModule } from '@ng-select/ng-select';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ConfirmModalComponent } from './components/modal/confirm-modal/confirm-modal.component';
import { HelpModalComponent } from './components/help-modal/help-modal.component';

@NgModule({
  declarations: [
    PagingComponent,
    PagingHeaderComponent,
    //NavComponent,
    TextInputComponent,
    DateInputComponent,
    ConfirmModalComponent,
    HelpModalComponent
  ],
  imports: [
    CommonModule,
    PaginationModule.forRoot(),
    CarouselModule.forRoot(),
    ReactiveFormsModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    RouterModule,
    NgSelectModule,
    ModalModule.forRoot()
    
  ],
  exports: [
    CommonModule,
    PagingComponent,
    PagingHeaderComponent,
    PaginationModule,
    CarouselModule,
    ReactiveFormsModule,
    FormsModule,
    BsDropdownModule,
    TextInputComponent,
    DateInputComponent,
    BsDatepickerModule,
    NgSelectModule,
    ModalModule
  ]
})
export class SharedModule { }
