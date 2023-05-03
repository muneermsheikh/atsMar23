import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { ICandidateAssessment } from 'src/app/shared/models/hr/candidateAssessment';
import { ICvAssessmentHeader } from 'src/app/shared/models/hr/cvAssessmentHeader';

@Component({
  selector: 'app-candidate-item',
  templateUrl: './candidate-item.component.html',
  styleUrls: ['./candidate-item.component.css']
})
export class CandidateItemComponent implements OnInit {

  @Input() cv: ICandidateBriefDto|undefined;
  @Output() msgEvent = new EventEmitter<number>();
  @Output() downloadEvent = new EventEmitter<number>();
  @Output() cvAssessEvent = new EventEmitter<ICandidateBriefDto>();
  @Output() cvCheckedEvent = new EventEmitter<ICandidateBriefDto>();
  @Output() cvEditEvent = new EventEmitter<number>();


  currentId=0;
  header: ICvAssessmentHeader|undefined;
  assessment: ICandidateAssessment|undefined;
  
  cvidForDocumentView: number=0;

  public isHidden: boolean = true;
  xPosTabMenu: number=0;
  yPosTabMenu: number=0;
  
  constructor() { }

  ngOnInit(): void {
  }
  //right click
  rightClick(event: any) {
    event.stopPropagation();
    this.xPosTabMenu = event.clientX;
    this.yPosTabMenu = event.clientY;
    this.isHidden = false;
    return false;
  }

  closeRightClickMenu() {
    this.isHidden = true;
  }

  download(id: number) {
    this.downloadEvent.emit(id);
  }

  async onClickLoadDocument(cvid: number) {
    // get a document from the Web API endpoint 'LoadDocument'
    this.msgEvent.emit(cvid);
  }

  setCurrentId(id: number) {
    this.currentId = id;
  }

  showhistory(cvid: number) {
    this.msgEvent.emit(cvid);
  }

  cvAssessClicked(t: ICandidateBriefDto)
  {
    this.cvAssessEvent.emit(t);
  }

  CVChecked(cv: ICandidateBriefDto) {
    this.cvCheckedEvent.emit(cv);
  }

  editCV(id: number) {
      this.cvEditEvent.emit(id);
  }

}
