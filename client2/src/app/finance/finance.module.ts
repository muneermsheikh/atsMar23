import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceIndexComponent } from './finance-index/finance-index.component';
import { CoaEditComponent } from './coa-edit/coa-edit.component';
import { CoaListComponent } from './coa-list/coa-list.component';
import { CoaItemComponent } from './coa-item/coa-item.component';
import { SoaComponent } from './soa/soa.component';
import { AddPaymentModalComponent } from './add-payment-modal/add-payment-modal.component';
import { CoaEditModalComponent } from './coa-edit-modal/coa-edit-modal.component';
import { ConfirmReceiptsComponent } from './confirm-receipts/confirm-receipts.component';
import { VoucherEditComponent } from './voucher-edit/voucher-edit.component';
import { VoucherItemComponent } from './voucher-item/voucher-item.component';
import { VoucherListComponent } from './voucher-list/voucher-list.component';
import { FinanceRoutingModule } from './finance-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SoaLineComponent } from './soa-line/soa-line.component';


@NgModule({
  declarations: [
    FinanceIndexComponent,
    CoaEditComponent,
    CoaListComponent,
    CoaItemComponent,
    SoaComponent,
    AddPaymentModalComponent,
    CoaEditModalComponent,
    ConfirmReceiptsComponent,
    VoucherEditComponent,
    VoucherItemComponent,
    VoucherListComponent,
    SoaLineComponent,

  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    FinanceRoutingModule
  ]
})
export class FinanceModule { }
