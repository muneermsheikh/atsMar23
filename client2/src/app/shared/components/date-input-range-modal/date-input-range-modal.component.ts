import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { DateInputRange } from '../../dtos/finance/dateInputRange';

@Component({
  selector: 'app-date-input-range-modal',
  templateUrl: './date-input-range-modal.component.html',
  styleUrls: ['./date-input-range-modal.component.css']
})
export class DateInputRangeModalComponent implements OnInit {

  @Input() returnDateRangeEvent = new EventEmitter();
  
  title: string = '';
  bsValue = new Date();
  bsRangeValue: Date = new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();
  //bsValueDateFirst = moment(new Date().getMonth() > 3 ? new Date(moment(new Date().getFullYear), 4,1) : moment(new Date().getFullYear)
  //bsModalRef: BsModalRef;
  
  constructor(private toastr: ToastrService, public bsModalRef: BsModalRef) { 
    this.minDate.setDate(this.minDate.getDate()-30);
  }

  ngOnInit(): void {
  }

  returnDateRange() {
    if(this.minDate > this.maxDate)  {
      this.toastr.warning('End Date should be equal to or later than from Date');
      return;
    }

    var dto = new DateInputRange();
    dto.fromDate=this.minDate;
    dto.uptoDate=this.maxDate;
    
    this.returnDateRangeEvent.emit(dto);
    
    this.bsModalRef.hide();

  }
}
