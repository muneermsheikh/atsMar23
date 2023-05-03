import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IEmployment } from 'src/app/shared/models/admin/selectionDecision';

@Component({
  selector: 'app-employment-modal',
  templateUrl: './employment-modal.component.html',
  styleUrls: ['./employment-modal.component.css']
})
export class EmploymentModalComponent implements OnInit {

  @Input() editEvent = new EventEmitter();
  
  emp?: IEmployment;
  title: string='';
  

  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  emitEmployment() {
    this.editEvent.emit(this.emp);
    this.bsModalRef.hide();
  }

}
