import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IEmployment } from 'src/app/shared/models/admin/employment';


@Component({
  selector: 'app-employment-line',
  templateUrl: './employment-line.component.html',
  styleUrls: ['./employment-line.component.css']
})
export class EmploymentLineComponent implements OnInit {

  //emp: Observable<IEmployment>;
  @Input() emp: IEmployment | undefined;
  @Output() editEvent = new EventEmitter<IEmployment>();

  empCopy: IEmployment | undefined;

  //@Input() data: Observable<any>;

  constructor(
    private toastr: ToastrService
    //, private cd: ChangeDetectorRef
    ) { }

  ngOnInit(): void {
    
   this.empCopy=this.emp;
  }

  isModified() {
    //console.log('copy:',this.empCopy.weeklyWorkHours, 'original:', this.emp.weeklyWorkHours);
    /*
    return (this.empCopy.weeklyWorkHours !== this.emp.weeklyWorkHours
        || this.empCopy.leavePerYearInDays !== this.emp.leavePerYearInDays
	      || this.empCopy.leaveAirfareEntitlementAfterMonths !==this.emp.leaveAirfareEntitlementAfterMonths 
        || this.empCopy.offerAcceptedOn !== this.emp.offerAcceptedOn
        || this.empCopy.charges !== this.emp.charges
	      || this.empCopy.salaryCurrency !== this.emp.salaryCurrency
	      || this.empCopy.salary !== this.emp.salary
	      || this.empCopy.contractPeriodInMonths !== this.emp.contractPeriodInMonths
	      || this.empCopy.housingProvidedFree !== this.emp.housingProvidedFree
        || this.empCopy.housingAllowance !== this.emp.housingAllowance
	      || this.empCopy.foodProvidedFree !== this.emp.foodProvidedFree
	      || this.empCopy.foodAlowance !== this.emp.foodAlowance
	      || this.empCopy.transportProvidedFree !== this.emp.transportProvidedFree
	      || this.empCopy.transportAllowance !== this.emp.transportAllowance
	      || this.empCopy.otherAllowance !== this.emp.otherAllowance
    )
    */
  }

  update() {
    this.editEvent.emit(this.emp);
  }

}
