import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { RegisterComponent } from '../account/register/register.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesRegisterGuard implements CanDeactivate<unknown> {

  canDeactivate(component: RegisterComponent) :boolean {
    if (component.registerForm!.dirty!) {
      return confirm('This form has unsaved data. Moving away from this form without saving data will result in loss of edited data.  Do you want to continue?');
    }
    return true;
  }
  
}
