import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICVReferredDto } from 'src/app/shared/dtos/admin/cvReferredDto';

@Component({
  selector: 'app-cvreferredline',
  templateUrl: './cvreferredline.component.html',
  styleUrls: ['./cvreferredline.component.css']
})
export class CvreferredlineComponent implements OnInit {

  @Input() referred: ICVReferredDto | undefined;
  @Output() remindClientForStatusEvent = new EventEmitter<number[]>();

  cvrefids: number[]=[];

  constructor() { }

  ngOnInit(): void {
  }

  displayChecklistModal() {

  }

  remindClientForStatus() {
    this.remindClientForStatusEvent.emit(this.cvrefids);
  }
}
