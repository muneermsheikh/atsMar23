import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrComponentlessModule } from 'ngx-toastr';
import { IUser } from 'src/app/shared/models/admin/user';
import { ConfirmService } from 'src/app/shared/services/confirm.service';

@Component({
  selector: 'app-master-edit-modal',
  templateUrl: './master-edit-modal.component.html',
  styleUrls: ['./master-edit-modal.component.css']
})
export class MasterEditModalComponent implements OnInit {
  @Input() editedStringName = new EventEmitter();
  user?: IUser;
  
  title: string='';
  caption: string='';
  returnString: string='';

  constructor(public bsModalRef: BsModalRef, private confirmService: ConfirmService) { }

  ngOnInit(): void {

  }

  close(){
    this.bsModalRef.hide();
  }
  
  updateString() {
    this.confirmService.confirm("Confirm change value", "Confirm if you want to change the '" + 
      this.caption + "' value").subscribe({
        next: response => {
          if(response) {
            this.editedStringName.emit(this.returnString);
            this.bsModalRef.hide();
          }
        }
      });
  }


}
