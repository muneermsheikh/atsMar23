import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { IndustryTypesComponent } from './industry-types/industry-types.component';


const routes = [
  {path: '', component: CategoriesComponent}, 
  {path: 'Qualifications', component:QualificationsComponent , data: {breadcrumb: {alias: 'qualifications'}}},
  {path: 'IndustryTypes', component:IndustryTypesComponent , data: {breadcrumb: {alias: 'Industry Types'}}},
  /*
  {path: 'clients', component:ClientsComponent , data: {breadcrumb: {alias: 'Customers'}}},
  {path: 'associates', component:ClientsComponent , data: {breadcrumb: {alias: 'Associates'}}},
  {path: 'vendors', component:ClientsComponent , data: {breadcrumb: {alias: 'Vendors'}}},
  {path: 'clients/:id', component:ClientEditComponent , data: {breadcrumb: {alias: 'Clients Edit'}}},
  */
]


@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
   ],
   exports: [
     RouterModule
   ]
})
export class MastersRoutingModule { }
