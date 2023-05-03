import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { IJDDto } from 'src/app/shared/dtos/admin/jdDto';
import { OrderService } from '../order.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-jd-modal',
  templateUrl: './jd-modal.component.html',
  styleUrls: ['./jd-modal.component.css']
})
export class JdModalComponent implements OnInit {

  @Input() updateSelectedJD = new EventEmitter();
  //jds: any;
  title: string='';
  jd?: IJDDto;

  closeBtnName: string='';

  form: FormGroup= new FormGroup({});
    
  //jd: IJobDescription;

  constructor(private service: OrderService, public bsModalRef: BsModalRef, private fb: FormBuilder ) {
   }

  ngOnInit(): void {
    //this.createForm();
    //this.form.patchValue(this.bsModalRef.content);
  }

  confirm() {
    
    this.updateSelectedJD.emit(this.jd);

    this.bsModalRef.hide();
  }


  decline() {
    this.bsModalRef.hide();
  }


}
