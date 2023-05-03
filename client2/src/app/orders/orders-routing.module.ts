import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrdersIndexComponent } from './orders-index/orders-index.component';
import { CategoryListResolver } from '../resolvers/categoryListResolver';
import { AgentsResolver } from '../resolvers/agents.resolver';
import { OrderEditComponent } from './order-edit/order-edit.component';
import { OrderResolver } from '../resolvers/orderResolver';
import { OrdersCreateGuard } from '../guards/ordersCreate.guard';
import { EmployeeIdsAndKnownAsResolver } from '../resolvers/employeeIdsAndKnownAsResolver';
import { CustomerNameCityResolver } from '../resolvers/customerNameCityResolver';
import { CustomerOfficialsResolver } from '../resolvers/customerOfficialsResolver';
import { AssessQComponent } from '../hr/assess-q/assess-q.component';
import { HrGuard } from '../guards/hr.guard';
import { OrderItemBriefResolver } from '../resolvers/orderItemBriefResolver';
import { AssessmentQsResolver } from '../resolvers/assessmentQsResolver';
import { ForwardsComponent } from './forwards/forwards.component';
import { DLForwardsOfAnOrderIdResolver } from '../resolvers/dlForwardsOfAnOrderIdResolver';


const routes = [
  {path: '', component: OrdersIndexComponent,  data: {breadcrumb: 'Admin Division'}},

  {path: 'edit/:id', component: OrderEditComponent,
    resolve: 
      {
        order: OrderResolver,
        agents: AgentsResolver,
        professions: CategoryListResolver,
        associates: CustomerOfficialsResolver,
        employees: EmployeeIdsAndKnownAsResolver,
        customers: CustomerNameCityResolver,
      },
      data: {breadcrumb: {alias: 'OrderEdit'}}
  },

  {path: 'add', component:OrderEditComponent , canActivate: [OrdersCreateGuard],
  resolve: {
      professions: CategoryListResolver,
      employees: EmployeeIdsAndKnownAsResolver,
      customers: CustomerNameCityResolver,
    },
    data: {breadcrumb: {alias: 'OrderAdd'}}},

  {path: 'view/:id', component: OrderEditComponent , data: {breadcrumb: {alias: 'OrderView'}}},

  {path: 'itemassess/:id', component: AssessQComponent,
    canActivate: [HrGuard],
    resolve: {
      itembrief: OrderItemBriefResolver,
      assessment: AssessmentQsResolver
  }},

    {path: 'forwards/:orderid', component: ForwardsComponent,
    data: {breadcrumb: {alias: 'DL Forwards'}},
    resolve: {
      dlforwarddata: DLForwardsOfAnOrderIdResolver }
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
export class OrdersRoutingModule { }
