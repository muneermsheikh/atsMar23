import { Component, OnInit } from '@angular/core';
import { employmentParams } from 'src/app/shared/params/admin/employmentParam';
import { EmploymentService } from '../employment.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employment-index',
  templateUrl: './employment-index.component.html',
  styleUrls: ['./employment-index.component.css']
})
export class EmploymentIndexComponent implements OnInit {

  notApproved=true;
  approved=false;
  orderNo=0;
  applicationNo=0;
  candidateName='';
  customername='';
  selectionDateFrom=new Date('1900-01-01');
  selectionDateUpto=new Date('1900-01-01');

  empParam = new employmentParams();

  constructor(private service: EmploymentService, private router: Router) { }


  ngOnInit(): void {
  }

  showEmployments() {
    if(this.approved===true && this.notApproved===true) {
      this.empParam.approved="null";
    } else if(this.approved === true) {
      this.empParam.approved="true";
    } else if (this.notApproved===true) {
      this.empParam.approved="false";
    } else {
      this.empParam.approved="null";
    }

    if(this.orderNo > 0) this.empParam.orderNo = this.orderNo;
    if(this.applicationNo > 0) this.empParam.applicationNo = this.applicationNo;

    if(this.isValidDate(this.selectionDateFrom)) {
      if(this.selectionDateFrom.getFullYear() > 2000) this.empParam.selectionDateFrom=new Date(this.selectionDateFrom);
    }

    if(this.isValidDate(this.selectionDateUpto)) {
      if(this.selectionDateUpto.getFullYear() > 2000) this.empParam.selectionDateUpto=new Date(this.selectionDateUpto);
    }

    this.service.setEParams(this.empParam);
    
    this.router.navigateByUrl('/employments/list');

  }


  isValidDate(date: any) {
    return date && Object.prototype.toString.call(date) === "[object Date]" && !isNaN(date);
  }
}
