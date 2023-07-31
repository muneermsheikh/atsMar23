import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallListComponent } from './call-list/call-list.component';
import { RouterModule } from '@angular/router';
import { PendingCallRecordsOfLoggedInUserResolver } from '../resolvers/pendingCallRecordsOfLoggedInUserResolver';

const routes = [
  {path: '', component: CallListComponent,
    Resolvers: {
      pendingCallRecordsOfLoggedInUser: PendingCallRecordsOfLoggedInUserResolver
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
export class CallRecordsRoutingModule { }
