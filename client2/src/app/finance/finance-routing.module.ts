import { NgModule } from '@angular/core';
import { FinanceIndexComponent } from './finance-index/finance-index.component';
import { VoucherListComponent } from './voucher-list/voucher-list.component';
import { VoucherEditComponent } from './voucher-edit/voucher-edit.component';
import { FinanceVoucherResolver } from '../resolvers/finance/financeVoucherResolver';
import { CoaListComponent } from './coa-list/coa-list.component';
import { CoaEditComponent } from './coa-edit/coa-edit.component';
import { StatementOfAccountResolver } from '../resolvers/finance/statementOfAccountResolver';
import { SoaComponent } from './soa/soa.component';
import { ReceiptsPendingConfirmtionResolver } from '../resolvers/receiptsPendingConfirmationResolver';
import { ConfirmReceiptsComponent } from './confirm-receipts/confirm-receipts.component';
import { RouterModule, Routes } from '@angular/router';
import { COAListResolver } from '../resolvers/finance/COAListResolver';


const routes: Routes = [
  {path: '', component: FinanceIndexComponent, data: {breadcrumb: 'Finances'}},
  
  {path: 'voucherlist', component: VoucherListComponent , data: {breadcrumb: {alias: 'VoucherList'}},
  },

  {path: 'addvoucher', component: VoucherEditComponent , data: {breadcrumb: {alias: 'add New Voucher'}},
    resolve: {
      coas: COAListResolver
    }
  },

  {path: 'editfinancewithobject/:id', component: VoucherEditComponent, 
    resolve: {
      coas: COAListResolver,
      voucher: FinanceVoucherResolver
    },
      data: {breadcrumb: {alias: 'Edit Voucher'}}
  },

  {path: 'viewvoucher/:id', component: VoucherEditComponent , data: {breadcrumb: {alias: 'viewVoucher'}}},

  {path: 'coalist', component:CoaListComponent
      //, canActivate: [FinanceGuard]
      , data: {breadcrumb: {alias: 'Chart of Account'}}
  },
  
  {path: 'coaedit/:id', component: CoaEditComponent
      //, canActivate: [FinanceGuard]
      , data: {breadcrumb: {alias: 'Edit Chart Of Account'}}
  },
  
  {path: 'coaview/:id', component: CoaEditComponent
      //, canActivate: [FinanceGuard]
      , data: {breadcrumb: {alias: 'View Chart of Account'}}
  },

  {path: 'editcoawithobject', component: CoaEditComponent, 
      data: {breadcrumb: {alias: 'Edit Chart Of Account'}}
  },

  {path: 'addaccount', component: CoaEditComponent, data: {breadcrumb: {alias: 'add chart of account'}}},

  {path: 'statementofaccount/:id/:fromDate/:uptoDate', 
    resolve: {soa: StatementOfAccountResolver},
    component: SoaComponent, data: {breakcrumb: {alias: 'statement of account'}}},
  
  {path: 'receiptspendingconfirmation', 
    resolve: {confirmationsPending: ReceiptsPendingConfirmtionResolver},
    component: ConfirmReceiptsComponent, data: {breakcrumb: {alias: 'Receipt pending confirmations'}}},
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
export class FinanceRoutingModule { }
