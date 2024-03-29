import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { DeployListComponent } from './deploys/deploy-list/deploy-list.component';
import { CandidateEditComponent } from './candidates/candidate-edit/candidate-edit.component';

const routes: Routes = [
  {path: '', component: HomeComponent, data: {breadcrumb: 'Home'}},
  {path: 'login', component: LoginComponent, data: {breadcrumb: 'LogIn'}},
  {path: 'register', component: CandidateEditComponent, data: {breadcrumb: 'Register'}},
  {path: 'account', loadChildren: () => import('./account/account.module').then(mod => mod.AccountModule), data: {breadcrumb: {skip: true}}},

  {path: 'master values', loadChildren:() => import('./masters/masters.module').then(mod => mod.MastersModule), data: {breadcrumb: 'Master Values'}},
  
  {path: 'candidates', loadChildren:() => import('./candidates/candidates.module').then(mod => mod.CandidatesModule), data: {breadcrumb: 'Candidates'}},
  {path: 'hr', loadChildren:() => import('./hr/hr.module').then(mod => mod.HrModule), data: {breadcrumb: 'HR'}},
  /* {path: 'candidatelist', component: CandidatesListingComponent, data: {breadcrumb: 'Candidate Listing'}}, */
  
  {path: 'orders', loadChildren:() => import('./orders/orders.module').then(mod => mod.OrdersModule), 
      data: {breadcrumb: 'orders'}},
  {path: 'customers', loadChildren:() => import('./customers/customers.module').then(mod => mod.CustomersModule), 
      data: {breadcrumb: 'Customers'}},

  /* 
  {path: 'checkout', loadChildren: () => import('./checkout/checkout.module').then(mod => mod.CheckoutModule), data: {breadcrumb: 'checkout'}},
  */
  
  {path: 'userTask', loadChildren: () => import('./userTask/user-task.module').then(mod => mod.UserTaskModule), data: {breadcrumb: 'userTask'}},
  
  {path: 'admin', loadChildren:() => import('./admin/admin.module').then(mod => mod.AdminModule), 
    //canActivate: [AdminGuard], 
    data: {breadcrumb: 'Admin'}},
  
  {path: 'employments', loadChildren:() => import('./employments/employment.module').then(mod => mod.EmploymentModule),
    data: {breadcrumb: 'Employments'}},

  
  {path: 'callrecords', loadChildren:() => import('./callRecords/call-records.module').then(mod => mod.CallRecordsModule), 
    //canActivate: [AdminGuard], 
    data: {breadcrumb: 'Call Records'}},
  /*
  {path: 'qualifications',component: QualificationsComponent, canActivate: [AuthorizedGuard], data: {breadcrumb: 'Qualifications'}}, 
  {path: 'masters', loadChildren:() => import('./masters/masters.module')
      .then(mod => mod.MastersModule), data: {breadcrumb: 'Masters'}},
  */  
  {path: 'processing', loadChildren:() => import('./deploys/deploy.module').then(mod => mod.DeployModule), 
      data: {breadcrumb: 'Deployment process'}},

  /* {path: 'processing', component: DeployListComponent, data: {breadcrumb: 'process list'}}, */
  
  {path: 'prospectives', loadChildren:() => import('./prospectives/prospective.module').then(mod => mod.ProspectiveModule), 
      data: {breadcrumb: 'Prospective Candidates'}},
  /*
      {path: 'notfound', component: NotFoundComponent, data: {breadcrumb: 'not-found errr'}},
  {path: 'messages', component: MessagesComponent, data: {breadcrumb: 'messages for loggedin user'}},
  {path: 'teachers', component: TeachersComponent},
  {path: 'process', loadChildren:() => import('./process/process.module').then(mod => mod.ProcessModule), data: {breadcrumb: 'Process'}},
  {path: 'interviews', loadChildren:() => import('./interview/interview.module').then(mod => mod.InterviewModule), data: {breadcrumb: 'Interviews'}},
  {path: 'servererror',component: SeverErrorComponent},
  {path: 'testerrors',component: TestErrorComponent},
  */
  {path: 'finance', loadChildren:() => import('./finance/finance.module').then(mod => mod.FinanceModule) },
  
  {path: '**', redirectTo: 'not-found', pathMatch: 'full'}
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
