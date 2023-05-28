import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IdsModalComponent } from './ids-modal/ids-modal.component';
import { OrdersIndexComponent } from './orders-index/orders-index.component';
import { OrderEditComponent } from './order-edit/order-edit.component';
import { OrderLineComponent } from './order-line/order-line.component';
import { ReviewStatuNamePipe } from './review-statu-name.pipe';
import { ReviewItemStatusNamePipe } from './review-item-status-name.pipe';
import { SharedModule } from '../shared/shared.module';
import { JdModalComponent } from './jd-modal/jd-modal.component';
import { ContractReviewItemModalComponent } from './contract-review-item-modal/contract-review-item-modal.component';
import { RemunerationModalComponent } from './remuneration-modal/remuneration-modal.component';
import { ChooseAgentsModalComponent } from './choose-agents-modal/choose-agents-modal.component';
import { ForwardsComponent } from './forwards/forwards.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrdersRoutingModule } from './orders-routing.module';



@NgModule({
  declarations: [
    IdsModalComponent,
    OrdersIndexComponent,
    OrderEditComponent,
    OrderLineComponent,
    ReviewStatuNamePipe,
    ReviewItemStatusNamePipe,
    JdModalComponent,
    ContractReviewItemModalComponent,
    RemunerationModalComponent,
    ChooseAgentsModalComponent,
    ForwardsComponent,
    
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    OrdersRoutingModule
  ]
})
export class OrdersModule { }
