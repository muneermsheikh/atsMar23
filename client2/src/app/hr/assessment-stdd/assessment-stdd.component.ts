import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IAssessmentStandardQ } from 'src/app/shared/models/admin/assessmentStandardQ';
import { assessmentStddQParam } from 'src/app/shared/models/admin/assessmentStddQParam';
import { StddqsService } from '../stddqs.service';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-assessment-stdd',
  templateUrl: './assessment-stdd.component.html',
  styleUrls: ['./assessment-stdd.component.css']
})
export class AssessmentStddComponent implements OnInit {

  
  qParams = new assessmentStddQParam();
  stddqs: IAssessmentStandardQ[]=[];

  totalPoints=0;
  bolNavigationExtras=false;
  returnUrl = '/hr';
  user?: IUser;

  constructor(private activatedRoute: ActivatedRoute, 
      private router: Router,
      private service: StddqsService , private toastr: ToastrService) {
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              this.bolNavigationExtras=true;
              if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
            }
       }

  ngOnInit(): void {
    this.service.setQParams(this.qParams);
    this.activatedRoute.data.subscribe(data => { 
      this.stddqs = data.stddqs;
      this.totalPoints =  this.stddqs.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
    })
  }

  deletestddq(id: number) {
    this.service.deletestddq(id).subscribe(response => {
      this.toastr.success("successfully deleted the standard question");
    }, error => {
      this.toastr.error(error);
    })
  }

  close(){
    this.router.navigateByUrl(this.returnUrl);
  }

}
