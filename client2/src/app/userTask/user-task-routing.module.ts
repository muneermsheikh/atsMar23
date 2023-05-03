import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTaskComponent } from './user-task/user-task.component';
import { UserTaskEditComponent } from './user-task-edit/user-task-edit.component';
import { UserTaskLineComponent } from './user-task-line/user-task-line.component';
import { TaskReminderModalComponent } from './task-reminder-modal/task-reminder-modal.component';
import { TaskModalComponent } from './task-modal/task-modal.component';
import { SharedModule } from '../shared/shared.module';
import { EmployeeIdsAndKnownAsResolver } from '../resolvers/employeeIdsAndKnownAsResolver';
import { ContactResultsResolver } from '../resolvers/contactResultsResolver';
import { TaskTypeResolver } from '../resolvers/taskTypesResolver';
import { UserTaskResolver } from '../resolvers/userTaskResolver';
import { UserTaskFromIdResolver } from '../resolvers/userTaskFromIdResolver';
import { RouterModule } from '@angular/router';


const routes = [
  {path: '', component: UserTaskComponent, resolve: {paginatedTask: UserTaskResolver}},
  {path: 'add', component: UserTaskEditComponent,
    resolve: {
      employees: EmployeeIdsAndKnownAsResolver,
      taskTypes: TaskTypeResolver,
      contactResult: ContactResultsResolver
    },
    data: {breadcrumb: {alias: 'TaskAdd'}}},

  {path: 'edit/:id', component: UserTaskEditComponent, 
    resolve: {
      employees: EmployeeIdsAndKnownAsResolver,
      taskTypes: TaskTypeResolver,
      contactResult: ContactResultsResolver,
      task: UserTaskFromIdResolver
    },
    data: {breadcrumb: {alias: 'TaskEdit'}}
  },

  {path: 'edittaskwithorderidandtasktype/:orderid/:tasktypeid', 
      resolve: {
        //task: TaskWithOrderIdAndTaskTypeResolver,
        employees: EmployeeIdsAndKnownAsResolver,
        taskTypes: TaskTypeResolver,
        contactResult: ContactResultsResolver
      },
      component: UserTaskEditComponent, data: {breadcrumb: {alias: 'TaskEdit'}}},
  {path: 'editwithobject', component: UserTaskEditComponent, data: {breadcrumb: {alias: 'TaskEdit'}}},
  {path: 'view/:id', component: UserTaskEditComponent , data: {breadcrumb: {alias: 'TaskView'}}},
  {path: 'viewbyresumeid/:resumeid', component: UserTaskEditComponent , data: {breadcrumb: {alias: 'TaskView'}}}
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
export class UserTaskRoutingModule { }
