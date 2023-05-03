import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { CvAssessComponent } from '../hr/cv-assess/cv-assess.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  canDeactivate(component: CvAssessComponent) :boolean {
    if (component.form!.dirty!) {
      return confirm('This form has unsaved data. Moving away from this form without saving data will result in loss of edited data.  Do you want to continue?');
    }
    return true;
  }
}
