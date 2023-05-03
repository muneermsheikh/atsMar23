import { Component, OnInit } from '@angular/core';
import { IHelp } from '../../models/admin/help';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-help-modal',
  templateUrl: './help-modal.component.html',
  styleUrls: ['./help-modal.component.css']
})
export class HelpModalComponent implements OnInit {

  help: IHelp | undefined;
  
  constructor(public bsModalRef: BsModalRef) { }
  
  ngOnInit(): void {
  }

}
