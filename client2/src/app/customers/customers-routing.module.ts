import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { RouterModule } from '@angular/router';
import { CustomerNameCityResolver } from '../resolvers/customerNameCityResolver';
import { IndustriesResolver } from '../resolvers/industriesResolver';
import { CustomersBriefResolver } from '../resolvers/customersBriefResolver';
import { ClientEditComponent } from './clients/client-edit/client-edit.component';
import { CustomerResolver } from '../resolvers/customerResolver';

const routes = [
  {path: '', component: CustomerListComponent,
    resolvers: {
      customerCities: CustomerNameCityResolver,
      industryTypes: IndustriesResolver,
      customers: CustomersBriefResolver
    }},

  {path: 'edit/:id', component: ClientEditComponent,
    resolvers: {
      customer: CustomerResolver,
      industryTypes: IndustriesResolver
    },
    data: {breadcrumb: {alias: 'Edit Customer/Associates'}}
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
export class CustomersRoutingModule { }
