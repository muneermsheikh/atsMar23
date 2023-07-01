import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeployListComponent } from './deploy-list/deploy-list.component';
import { DeployIndexComponent } from './deploy-index/deploy-index.component';
import { RouterModule } from '@angular/router';
import { DeployEditComponent } from './deploy-edit/deploy-edit.component';
import { CVsRefWithDeploysResolver } from '../resolvers/cvsRefWithDeploysResolver';

const routes = [
  {path: '', component: DeployIndexComponent},
  {path: 'list', component: DeployListComponent},

  {path: 'list/:orderid', component: DeployListComponent},
  {path: 'edit/:cvrefid', component: DeployEditComponent,
    resolve: {
      deployment: CVsRefWithDeploysResolver,
      //depStatuses: DeploymentStatusResolver
    }}
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

export class DeployRoutingModule { }
