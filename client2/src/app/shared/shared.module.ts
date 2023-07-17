import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagingComponent } from './components/paging/paging.component';
import { PagingHeaderComponent } from './components/paging-header/paging-header.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { TabsModule } from 'ngx-bootstrap/tabs';
import { customerStatusPipe } from './pipes/customer-status.pipe';
import { DepStatusPipe } from './pipes/dep-status.pipe';
import { InputModalComponent } from './components/input-modal/input-modal.component';
import { DateInputRangeModalComponent } from './components/date-input-range-modal/date-input-range-modal.component';


@NgModule({
  declarations: [
    PagingComponent,
    PagingHeaderComponent,
    //NavComponent,
    TextInputComponent,
    DateInputComponent,
    ConfirmModalComponent,
    HelpModalComponent,
    customerStatusPipe,
    DepStatusPipe,
    InputModalComponent,
    DateInputRangeModalComponent,
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
    ModalModule.forRoot(),
    TabsModule
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
    ModalModule,
    TabsModule,
    customerStatusPipe,
    DepStatusPipe
  ]
})
export class SharedModule { }
