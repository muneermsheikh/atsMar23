import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IUserHistoryDto } from 'src/app/shared/dtos/admin/userHistoryDto';

@Component({
  selector: 'app-call-item',
  templateUrl: './call-item.component.html',
  styleUrls: ['./call-item.component.css']
})
export class CallItemComponent implements OnInit {

  @Output() editEvent = new EventEmitter<IUserHistoryDto>();
  @Output() deleteEvent = new EventEmitter<number>();
  
  @Input() hist?: IUserHistoryDto;
  
  constructor() { }

  ngOnInit(): void {
  }

  editClicked() {
    console.log('edit clicked');
    this.editEvent.emit(this.hist);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.hist?.id);
  }

}
