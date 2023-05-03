import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IOrderBriefDto } from 'src/app/shared/dtos/admin/orderBriefDto';

@Component({
  selector: 'app-order-line',
  templateUrl: './order-line.component.html',
  styleUrls: ['./order-line.component.css']
})
export class OrderLineComponent implements OnInit {

  @Input() order: IOrderBriefDto | undefined;

  @Output() viewEvent = new EventEmitter<number>();
  @Output() editEvent = new EventEmitter<number>();
  @Output() contractReviewEvent = new EventEmitter<number>();
  @Output() dlFwdToHREvent = new EventEmitter<number>();
  @Output() dlForwardToHREvent = new EventEmitter<number>();
  @Output() dlFwdToAssociatesEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  

  menuTopLeftPosition =  {x: 0, y: 0}

  constructor() { }

  ngOnInit(): void {
  }

  viewForwardedToHRClicked() {
    this.dlForwardToHREvent.emit(this.order!.id);
  }

  contractReviewClicked() {
    this.contractReviewEvent.emit(this.order!.id);
  }

  editClicked() {

    this.editEvent.emit(this.order!.id);
  }
  
  viewClicked() {
    this.viewEvent.emit(this.order!.id);
  }


  deleteClicked() {
    this.deleteEvent.emit(this.order?.id);
  }

  dlForwardToHRClicked() {
    this.dlForwardToHREvent.emit(this.order?.id);
    /*
    if (new Date(this.order!.forwardedToHRDeptOn).getFullYear() > 2000) {
      this.dlForwardedToHREvent.emit(this.order.id);
    } else {
      this.dlforwardService.forwardDLtoHRHead(this.order.id).subscribe(response => {
        if(response!==null) {
          this.order.forwardedToHRDeptOn=response.taskDate;
          this.dlFwdToHREvent.emit('DL Forwarded to HR Department');
        } else {
          this.dlFwdToHREvent.emit('failed to forward the DL to HR Head');
        }
      })
    }
    */
  }
  

  dlForwardToAssociatesClicked() {
    this.dlFwdToAssociatesEvent.emit(this.order?.id);
  }

}
