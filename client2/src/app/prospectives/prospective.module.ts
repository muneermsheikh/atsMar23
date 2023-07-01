import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProspectiveComponent } from './prospective/prospective.component';
import { ProspectiveItemComponent } from './prospective-item/prospective-item.component';



@NgModule({
  declarations: [
    ProspectiveComponent,
    ProspectiveItemComponent
  ],
  imports: [
    CommonModule
  ]
})
export class ProspectiveModule { }
