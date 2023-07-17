import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from './categories/categories.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

import { QualificationsComponent } from './qualifications/qualifications.component';
import { IndustryTypesComponent } from './industry-types/industry-types.component';
import { ClientsComponent } from '../customers/clients/clients.component';
import { ClientEditComponent } from '../customers/clients/client-edit/client-edit.component';

import { MasterEditModalComponent } from './master-edit-modal/master-edit-modal.component';



@NgModule({
  declarations: [
    CategoriesComponent,

    QualificationsComponent,
    IndustryTypesComponent,
    ClientsComponent,
    ClientEditComponent,
    

    MasterEditModalComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class MastersModule { }
