import { NgModule } from '@angular/core';
import { SectionHeaderComponent } from './section-header/section-header.component';

import { NavComponent } from '../shared/components/nav/nav.component';
import { RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { BreadcrumbModule } from 'xng-breadcrumb';
import { NgxSpinnerModule } from 'ngx-spinner';
import { SharedModule } from '../shared/shared.module';
import { CommonModule } from '@angular/common';
import { ServerErrorComponent } from './server-error/server-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TestErrorComponent } from './test-error/test-error.component';



@NgModule({
  declarations: [
    NavComponent,
    SectionHeaderComponent,
    ServerErrorComponent,
    NotFoundComponent,
    TestErrorComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ToastrModule.forRoot(
      {
        positionClass: 'toast-bottom-right',
        preventDuplicates: true
      }
    ),
    BreadcrumbModule,
    NgxSpinnerModule,
    SharedModule
  ],
  exports: [
    NavComponent,
    SectionHeaderComponent,
    NgxSpinnerModule
  ]
})
export class CoreModule { }
