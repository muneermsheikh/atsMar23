import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IAssessmentStandardQ } from 'src/app/shared/models/admin/assessmentStandardQ';
import { assessmentStddQParam } from 'src/app/shared/models/admin/assessmentStddQParam';
import { StddqsService } from '../stddqs.service';

@Component({
  selector: 'app-assessment-stdd',
  templateUrl: './assessment-stdd.component.html',
  styleUrls: ['./assessment-stdd.component.css']
})
export class AssessmentStddComponent implements OnInit {

  
  qParams = new assessmentStddQParam();
  stddqs?: IAssessmentStandardQ[];

  constructor(private activatedRoute: ActivatedRoute, 
      private service: StddqsService , private toastr: ToastrService) { }

  ngOnInit(): void {
    this.service.setQParams(this.qParams);
    this.activatedRoute.data.subscribe(data => { 
      this.stddqs = data.stddqs;
    })
  }

  deletestddq(id: number) {
    this.service.deletestddq(id).subscribe(response => {
      this.toastr.success("successfully deleted the standard question");
    }, error => {
      this.toastr.error(error);
    })
  }

}
