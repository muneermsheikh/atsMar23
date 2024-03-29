import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { RouterModule } from '@angular/router';
import { IndustriesResolver } from '../resolvers/industriesResolver';
import { CustomersBriefResolver } from '../resolvers/customersBriefResolver';
import { ClientEditComponent } from './clients/client-edit/client-edit.component';
import { CustomerResolver } from '../resolvers/customerResolver';
import { CustomersIndexComponent } from './customers-index/customers-index.component';
import { CategoryListResolver } from '../resolvers/categoryListResolver';

const routes = [
  {path: '', component: CustomersIndexComponent,
    /*resolvers: {
      customerCities: CustomerNameCityResolver,
      industryTypes: IndustriesResolver,
      customers: CustomersBriefResolver
    } */
  },
  
  {path: 'customerlist/:custType', component: CustomerListComponent,
    resolvers: {
      //customerCities: CustomerNameCityResolver,
      //industryTypes: IndustriesResolver,
      customers: CustomersBriefResolver
    }},

  {
    path: 'edit/:id', component: ClientEditComponent,
    data: {breadcrumb: {alias: 'Edit Customer/Associates'}}
  },

  {
    path: 'add/:custType', component: ClientEditComponent,
    data: {breadcrumb: {alias: 'Edit Customer/Associates'}}
},
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
export class CustomersRoutingModule { }
