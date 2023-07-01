import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmploymentListComponent } from './employment-list/employment-list.component';
import { RouterModule } from '@angular/router';
import { EmploymentIndexComponent } from './employment-index/employment-index.component';
import { EmploymentsResolver } from '../resolvers/employmentsResolver';
import { EmploymentModalComponent } from '../admin/employment-modal/employment-modal.component';

const routes = [
  {path: '', component: EmploymentIndexComponent, data: {breadcrumb: 'Employment Index' }},

  {path: 'list', component: EmploymentListComponent,
      resolvers: {
      employments: EmploymentsResolver
  }},
  {path: 'edit/:id', component: EmploymentModalComponent  },
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  
  exports: [
    RouterModule
  ]
})

export class EmploymentRoutingModule { }
