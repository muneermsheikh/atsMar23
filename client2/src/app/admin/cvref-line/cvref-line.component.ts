import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
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

  constructor(private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  cvChecked(checked: boolean) {
    
    /* if(this.cv?.assessedResult!=='agreed') {
      this.toastr.warning('candidates whose status on charges shows as "agreed" can only be selected');
      return;
    }
    */

    var cvid = this.cv!.id;
    if(checked===undefined) checked=true;
    //checked=!checked;
    this.checkedNBoolean.checked=checked;
    this.checkedNBoolean.id=cvid;
    //console.log('cvhecked in line', this.checkedNBoolean);
    this.cvCheckedEvent.emit(this.checkedNBoolean);
  }


  displayCV() {
    
  }

  showHRChecklist() {
    this.displayChecklistModalEvent.emit(this.cv);
  }
}
