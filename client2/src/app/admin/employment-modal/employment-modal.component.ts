import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IEmployment } from 'src/app/shared/models/admin/employment';
import { IUser } from 'src/app/shared/models/admin/user';


@Component({
  selector: 'app-employment-modal',
  templateUrl: './employment-modal.component.html',
  styleUrls: ['./employment-modal.component.css']
})
export class EmploymentModalComponent implements OnInit {

  @Input() editEvent = new EventEmitter();
  
  emp?: IEmployment;
  user?: IUser;
  

  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  emitEmployment() {
    this.editEvent.emit(this.emp);
    this.bsModalRef.hide();
  }

  approvedClicked() {

    if(this.emp !== undefined && new Date(this.emp.approvedOn).getFullYear() < 2000){
      this.emp.approvedOn = new Date();
      this.toastr.info('date set');
    } 
  }
  
}
