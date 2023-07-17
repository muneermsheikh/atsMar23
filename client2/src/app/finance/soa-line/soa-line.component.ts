import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IStatementofAccountItemDto } from 'src/app/shared/dtos/finance/statementOfAccountDto';

@Component({
  selector: 'app-soa-line',
  templateUrl: './soa-line.component.html',
  styleUrls: ['./soa-line.component.css']
})
export class SoaLineComponent implements OnInit {

  @Input() soaItem: IStatementofAccountItemDto | undefined;
  @Output() displayVoucherEvent = new EventEmitter<number>();
  
  constructor() { }

  ngOnInit(): void {
  }

  displayVoucher(){
    this.displayVoucherEvent.emit(this.soaItem!.voucherNo);
  }

}
