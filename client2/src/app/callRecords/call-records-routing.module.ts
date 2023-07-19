import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallListComponent } from './call-list/call-list.component';
import { RouterModule } from '@angular/router';

const routes = [
  {path: '', component: CallListComponent}
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
