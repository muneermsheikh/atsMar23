import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICOA } from 'src/app/shared/models/masters/finance/coa';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-coa-item',
  templateUrl: './coa-item.component.html',
  styleUrls: ['./coa-item.component.css']
})
export class CoaItemComponent implements OnInit {

  @Input() t?: ICOA;

  @Output() editEvent = new EventEmitter<ICOA>();
  @Output() viewEvent = new EventEmitter<ICOA>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() statementOfAccountEvent = new EventEmitter<number>();
  @Output() updateCandidateAccountEvent = new EventEmitter<ICOA>();

  constructor(private confirmService: ConfirmService) { }

  ngOnInit(): void {
  }

  statementOfAccount(id: number) {
    this.statementOfAccountEvent.emit(id);
  }

  updateCandidateAccountName() {
    this.updateCandidateAccountEvent.emit(this.t);
  }

  updateCOA(t: ICOA) {
    this.editEvent.emit(t);
  }

  viewCOA(t: ICOA) {
    this.viewEvent.emit(t);
  }
  
  deleteCOA(id: number) {

    this.confirmService
      .confirm('deleting this account will also delete all its transactions.  Do you really want to continue with the deletion?', 
        'confirm DELETE a chart of account along with all its transactions').subscribe(response => {
          if(!response) {
            return;
          }
        })
    
    this.deleteEvent.emit(id);
  }

  deleteTransaction(id: number) {
    
  }

  editCOA() {
    this.editEvent.emit(this.t);
  }
}
