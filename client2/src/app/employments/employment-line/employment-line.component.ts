import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IEmployment } from 'src/app/shared/models/admin/employment';


@Component({
  selector: 'app-employment-line',
  templateUrl: './employment-line.component.html',
  styleUrls: ['./employment-line.component.css']
})
export class EmploymentLineComponent implements OnInit {

  @Input() employment: IEmployment | undefined;
  @Output() editEvent = new EventEmitter<IEmployment>();
  @Output() deleteEvent = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

  editEmployment() {
    this.editEvent.emit(this.employment);
  }

  deleteEmployment() {
    this.deleteEvent.emit(this.employment?.id);
  }

}
