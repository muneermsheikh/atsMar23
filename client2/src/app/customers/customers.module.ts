import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerItemComponent } from './customer-item/customer-item.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { CustomersRoutingModule } from './customers-routing.module';
import { CustomersIndexComponent } from './customers-index/customers-index.component';
import { TabsModule } from 'ngx-bootstrap/tabs';


@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerItemComponent,
    CustomersIndexComponent,
   
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    CustomersRoutingModule,
    SharedModule,
    TabsModule
  ]
})
export class CustomersModule { }
