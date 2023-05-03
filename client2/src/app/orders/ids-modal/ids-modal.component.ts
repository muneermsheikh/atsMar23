import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IOrderItemBriefDto } from 'src/app/shared/dtos/admin/orderItemBriefDto';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-ids-modal',
  templateUrl: './ids-modal.component.html',
  styleUrls: ['./ids-modal.component.css']
})
export class IdsModalComponent implements OnInit {

  @Input() emitterObj = new EventEmitter();
  user?: IUser;
  title='';
  orderItems: IOrderItemBriefDto[]=[];
  ids: number[]=[];
  
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  emitSelectedIds() {
    this.emitterObj.emit(this.ids);
    this.bsModalRef.hide();
  }

}
