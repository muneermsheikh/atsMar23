import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { IEmploymentDto } from 'src/app/shared/dtos/admin/employmentDto';

@Component({
  selector: 'app-employment',
  templateUrl: './employment.component.html',
  styleUrls: ['./employment.component.css']
})
export class EmploymentComponent implements OnInit {

  employmentFormValue = new EventEmitter<any>()
  @Input() employments: IEmploymentDto[]=[];
  
  form: FormGroup = new FormGroup({});
  errors: string[]=[];

  constructor(private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.createForm();
    //if (this.employments.length > 0) this.patchForm(this.employment);
  }


  createForm() {
    this.form = this.fb.group({
        id: 0, cVRefId: 0, selectionDecisionId: 0, selectedOn: '', 
        charges: 0, candidateName: '', customerName: '', categoryRef: '',
        salaryCurrency: '', salary: 0, contractPeriodInMonths: 24, 
        housingProvidedFree: false, housingAllowance: 0, 
        foodProvidedFree: false, foodAlowance: 0, 
        transportProvidedFree: false, transportAllowance: 0,
        otherAllowance: 0, leavePerYearInDays: 21,
        leaveAirfareEntitlementAfterMonths: 24, offerAcceptedOn: '',
        remarks: ''
    })
  }

  patchForm(emps: IEmploymentDto[]) {
    emps.forEach(emp => {
        this.form.patchValue( {
          id: emp.id, cVRefId: emp.cVRefId, selectionDecisionId: emp.selectionDecisionId, 
          selectedOn: emp.selectedOn, charges: emp.charges, candidateName: emp.candidateName, 
          customerName: emp.companyName, categoryRef: emp.categoryRef,
          salaryCurrency: emp.salaryCurrency, salary: emp.salary, 
          contractPeriodInMonths: emp.contractPeriodInMonths, 
          housingProvidedFree: emp.housingProvidedFree, housingAllowance: emp.housingAllowance, 
          foodProvidedFree: emp.foodProvidedFree, foodAlowance: emp.foodAlowance, 
          transportProvidedFree: emp.transportProvidedFree, transportAllowance: emp.transportAllowance,
          otherAllowance: emp.otherAllowance, leavePerYearInDays: emp.leavePerYearInDays,
          leaveAirfareEntitlementAfterMonths: emp.leaveAirfareEntitlementAfterMonths, offerAcceptedOn: emp.offerAcceptedOn,
          remarks: emp.remarks
      })
    })
  }

  onSubmit() {

    this.employmentFormValue.emit(this.form.value);
    this.router.navigateByUrl('/seldecision');
  }

}
