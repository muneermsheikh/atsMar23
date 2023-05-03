import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { SharedModule } from '../shared/shared.module';
import { RegisterComponent } from './register/register.component';
import { UserNamesComponent } from './user-names/user-names.component';
import { UserAddressComponent } from './user-address/user-address.component';
import { UserPhonesComponent } from './user-phones/user-phones.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FileUploaderComponent } from './file-uploader/file-uploader.component';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    UserNamesComponent,
    UserAddressComponent,
    UserPhonesComponent,
    FileUploaderComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TabsModule,
  ]
})
export class AccountModule { }
