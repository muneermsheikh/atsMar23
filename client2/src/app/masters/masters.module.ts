import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from './categories/categories.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { CategoryEditModalComponent } from './category-edit-modal/category-edit-modal.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { IndustryTypesComponent } from './industry-types/industry-types.component';
import { ClientsComponent } from '../customers/clients/clients.component';
import { ClientEditComponent } from '../customers/clients/client-edit/client-edit.component';
import { CategoryEditComponent } from './category-edit/category-edit.component';
import { MasterEditComponent } from './master-edit/master-edit.component';



@NgModule({
  declarations: [
    CategoriesComponent,
    CategoryEditModalComponent,
    QualificationsComponent,
    IndustryTypesComponent,
    ClientsComponent,
    ClientEditComponent,
    CategoryEditComponent,
    MasterEditComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class MastersModule { }
