import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskModalComponent } from './task-modal/task-modal.component';
import { UserTaskComponent } from './user-task/user-task.component';
import { UserTaskEditComponent } from './user-task-edit/user-task-edit.component';
import { UserTaskLineComponent } from './user-task-line/user-task-line.component';
import { TaskReminderModalComponent } from './task-reminder-modal/task-reminder-modal.component';
import { SharedModule } from '../shared/shared.module';
import { UserTaskRoutingModule } from './user-task-routing.module';



@NgModule({
  declarations: [
    UserTaskComponent,
    UserTaskEditComponent,
    UserTaskLineComponent,
    TaskReminderModalComponent,
    TaskModalComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    UserTaskRoutingModule
  ]
})
export class UserTaskModule { }
