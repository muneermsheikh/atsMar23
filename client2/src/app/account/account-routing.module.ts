import { NgModule } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { RouterModule, Routes } from '@angular/router';
import { CategoryListResolver } from '../resolvers/categoryListResolver';
import { CandidateResolver } from '../resolvers/candidateResolver';
import { AgentsResolver } from '../resolvers/agents.resolver';

const routes: Routes = [
  {path: '', component: LoginComponent,  data: {breadcrumb: 'Login Admin'}},
  {path: 'login', component: LoginComponent,data: {breadcrumb: {alias: 'logIn'}}},
  /*{path: 'register', component: RegisterComponent, 
      resolve: {
        categories: CategoryListResolver,
        agents: AgentsResolver,
        //qualifications: QualificationListResolver,
        candidate: CandidateResolver
      },
      data: {breadcrumb: {alias: 'register'}}
  },
  {path: 'edit/:id', component: RegisterComponent, 
      resolve: {
        //qualifications: QualificationListResolver,
        agents: AgentsResolver,
        categories: CategoryListResolver,
        candidate: CandidateResolver
      },
      data: {breadcrumb: {alias: 'Edit Or Add'}}
},*/
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
export class AccountRoutingModule { }
