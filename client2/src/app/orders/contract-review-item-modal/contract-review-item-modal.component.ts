import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IContractReviewItem } from 'src/app/shared/models/admin/contractReviewItem';
import { IReviewItemStatus } from 'src/app/shared/models/admin/reviewItemStatus';

@Component({
  selector: 'app-contract-review-item-modal',
  templateUrl: './contract-review-item-modal.component.html',
  styleUrls: ['./contract-review-item-modal.component.css']
})
export class ContractReviewItemModalComponent implements OnInit {

  @Input() updateModalReview = new EventEmitter();
  review?: IContractReviewItem;
  reviewStatus: IReviewItemStatus[]=[];
  
  form: FormGroup = new FormGroup({});  
  
  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder ) { }

  ngOnInit(): void {
  }

  confirm() {
    this.updateModalReview.emit(this.review);
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }

}
