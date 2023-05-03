import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-category-edit-modal',
  templateUrl: './category-edit-modal.component.html',
  styleUrls: ['./category-edit-modal.component.css']
})
export class CategoryEditModalComponent implements OnInit {

  @Input() updateStringName = new EventEmitter();
  user?: IUser;
  
  str: string='';

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  
  updateString() {
    this.updateStringName.emit(this.str);
    this.bsModalRef.hide();
  }

}
