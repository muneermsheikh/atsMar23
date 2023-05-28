import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminIndexComponent } from './admin-index/admin-index.component';
import { SelectionComponent } from './selection/selection.component';
import { SelectionsGuard } from '../guards/selections.guard';
import { MessagesComponent } from './messages/messages.component';
import { DlForwardComponent } from './dl-forward/dl-forward.component';
import { AssociateForwardsForADLResolver } from '../resolvers/associateForwardsForADLResolver';
import { CategoriesComponent } from '../masters/categories/categories.component';
import { QualificationsComponent } from '../masters/qualifications/qualifications.component';
import { QualificationsResolver } from '../resolvers/qualificationsResolver';
import { PendingSelectionsResolver } from '../resolvers/pendingSelectionsResolver';
import { SelectionStatusResolver } from '../resolvers/selectionStatusResolver';
import { RouterModule } from '@angular/router';
import { CvrefComponent } from './cvref/cvref.component';
import { CategoryListResolver } from '../resolvers/categoryListResolver';
import { AgentsResolver } from '../resolvers/agents.resolver';


const routes = [
  {path: '', component: AdminIndexComponent,  data: {breadcrumb: 'Admin Division'}},
  {path: 'cvforward', component: CvrefComponent,
    resolve: 
      {
        //assessedcvs: AssessedAndApprovedCVsResolver,
        professions: CategoryListResolver,
        agents: AgentsResolver
      },
    data: {breadcrumb: {breadcrumb: 'CV Forward to clients'}}
  },
  
  {path: 'selections', component: SelectionComponent, canActivate: [SelectionsGuard], data: {breadcrumb: 'Selections'}},
  
  /* {path: 'employments', component: EmploymentsComponent, canActivate: [SelectionsGuard],
      resolve: {ordersbrief: OrdersBriefResolver },
      data: {breadcrumb: 'Employments'}},
  */
 
  {path: 'messages', component: MessagesComponent, canActivate: [SelectionsGuard], data: {breadcrumb: 'Email Messages'}},
  
  {path: 'forwarded/:orderid', component: DlForwardComponent, canActivate: [SelectionsGuard], data: {breadcrumb: 'DL Forwards to Agents'},
    resolve: {forwards: AssociateForwardsForADLResolver}  
  },
  {path: 'categories', component: CategoriesComponent, 
      canActivate: [SelectionsGuard], data: {breadcrumb: 'Categories'},
  //resolve: {cats: CategoriesResolver}  
  },
  {path: 'qualifications', component: QualificationsComponent, 
      //canActivate: [SelectionsGuard], 
      data: {breadcrumb: 'Qualificaions'},
      resolve: {quals: QualificationsResolver}  
  },
  
  {path: 'pendingselections', component: SelectionComponent, 
      canActivate: [SelectionsGuard], 
      data: {breadcrumb: 'Selections'}
      , resolve: {
        selectionsPending: PendingSelectionsResolver,
        selectionStatus: SelectionStatusResolver
      }  
  }
  
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


export class AdminRoutingModule { }
