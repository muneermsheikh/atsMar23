import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CheckedAndBoolean, ICheckedAndBoolean } from 'src/app/shared/dtos/admin/checkedAndBooean';
import { ICandidateAssessedDto } from 'src/app/shared/dtos/hr/candidateAssessedDto';

@Component({
  selector: 'app-cvref-line',
  templateUrl: './cvref-line.component.html',
  styleUrls: ['./cvref-line.component.css']
})
export class CvrefLineComponent implements OnInit {

  @Input() cv: ICandidateAssessedDto|undefined;
  @Output() cvCheckedEvent = new EventEmitter<ICheckedAndBoolean>();
  @Output() displayChecklistModalEvent = new EventEmitter<ICandidateAssessedDto>();

  checkedNBoolean = new CheckedAndBoolean();

  constructor() { }

  ngOnInit(): void {
  }

  cvChecked(cvid: number, checked: boolean) {
    this.checkedNBoolean.checked=checked;
    this.checkedNBoolean.id=cvid;
    this.cvCheckedEvent.emit(this.checkedNBoolean);
  }

  displayChecklistModal(item: ICandidateAssessedDto) {
    this.displayChecklistModalEvent.emit(item);
  }

}
