import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DeployRoutingModule } from './deploy-routing.module';
import { DeployListComponent } from './deploy-list/deploy-list.component';
import { DeployItemComponent } from './deploy-item/deploy-item.component';
import { DepModalComponent } from './dep-modal/dep-modal.component';
import { DeployIndexComponent } from './deploy-index/deploy-index.component';
import { EditDepComponent } from './edit-dep/edit-dep.component';
import { DeployEditComponent } from './deploy-edit/deploy-edit.component';
import { DeployAddModalComponent } from './deploy-add-modal/deploy-add-modal.component';



@NgModule({
  declarations: [
    DeployListComponent,
    DeployItemComponent,
    DepModalComponent,
    DeployIndexComponent,
    EditDepComponent,
    DeployEditComponent,
    DeployAddModalComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    DeployRoutingModule

  ]
})
export class DeployModule { }
