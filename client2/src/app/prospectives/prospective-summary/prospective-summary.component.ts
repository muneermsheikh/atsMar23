import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'src/app/account/account.service';
import { IUser } from 'src/app/shared/models/admin/user';
import { ProspectiveService } from '../prospective.service';
import { UserTaskService } from 'src/app/userTask/user-task.service';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { prospectiveSummaryParams } from 'src/app/shared/params/hr/prospectiveSummaryParams';
import { IProspectiveSummaryDto } from 'src/app/shared/dtos/hr/propectiveSummaryDto';
import { ITaskType } from 'src/app/shared/models/admin/taskType';
import { IApplicationTask } from 'src/app/shared/models/admin/applicationTask';

@Component({
  selector: 'app-prospective-summary',
  templateUrl: './prospective-summary.component.html',
  styleUrls: ['./prospective-summary.component.css']
})
export class ProspectiveSummaryComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  @ViewChild('searchCatRef', {static: false}) searchCatRef?: ElementRef;

  user?: IUser;

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService,
      private service: ProspectiveService, 
      private taskService: UserTaskService,
      private router: Router, 
      private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
   }

  sParams = new prospectiveSummaryParams();

  statusParams: string = 'pending';

  summary: IProspectiveSummaryDto[]=[];
  totalCount: number=0;

  taskTypes: ITaskType[]=[];
   taskType: number=0;
   personType: string='';
   taskStatus: string='';

  showDetails(st: string, cat: string, dt: string, no: number) {

    if(no===0) {
      this.toastr.warning('Status total is zero');
      return;
    }
    
    var params = this.service.getParams();
    params.status=st;
    params.categoryRef=cat;
    params.dateAdded = dt;
    this.service.setParams(params);
    //this.router.navigateByUrl('/prospectives/prospectivelist/' + cat + '/' + st);
    let route = '/prospectives/prospectivelist/' + cat  +'/' + '/' + dt + '/' + st;
    this.router.navigate([route], { state: { returnUrl: '/prospectives/true' } });
  }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.summary = data.summary;
      //this.totalCount = data.summary.count;
    })
  }

  getSummary(useCache=false) {
    return this.service.getProspectiveSummary(useCache).subscribe(response =>{
      this.summary = response;  //.data;
      //this.totalCount=response.count;
    }, error => {
      console.log('error', error);
    })
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new prospectiveSummaryParams();
    this.service.setSummaryParams(this.sParams);
    this.getSummary();
  }
  
  onPageChanged(event: any){
    const params = this.service.getSummaryParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setSummaryParams(params);
      this.getSummary(true);

    }
  }

  
  showProspectiveList(cat: string) {
    //this.router.navigateByUrl("/prospectives/prospectivelist/" + cat + '/' + '');

    let route = '/prospectives/prospectivelist' + cat +'/' + '';
    this.router.navigate([route], { state: { returnUrl: '/prospectives/true' } });

  }

  //search tasks
  /*
  personTypeClicked_CandidateClicked() {
    this.personType='candidate';
  }

  personTypeClicked_Official() {
    this.personType='official';
  }

  peronsTypeClicked_Employee() {
    this.personType='employee';
  }

  taskStatusClicked_Open() {
    this.taskStatus='open';
  }

  taskStatusClicked_Closed() {
    this.taskStatus="closed";
  }

  taskStatus_All() {
    this.taskStatus='';
  }

  FindTasks() {
    
  }
  */

  onSearch() {
    //app 10105, 10106.
    //phone nos.9862238066, 9867688066, place@gmail.com, rajesh@gmail.com

    var search = this.searchTerm!.nativeElement.value;

    if (search==='') return;

    var t:IApplicationTask;
    this.taskService.getTaskFromAny(search).subscribe(response => {
      t = response;

      if(t===null) {
        this.toastr.error('the value ' + search + ' did not return any task object');
        return;
      }
      let route = '/userTask/editwithobject';
      this.router.navigate([route], { state: { tasktoedit: t, returnUrl: '/prospectives/true' } });
    }, error => {
      console.log('error is:', error);
      this.toastr.info('your search text did not return any task');
      return;
    })

  }

  searchSummaryByCatRef() {
      var search = this.searchCatRef!.nativeElement.value;
      
      if(this.sParams.categoryRef===search) return;

      this.sParams.categoryRef=search;

      this.service.setSummaryParams(this.sParams);

      this.getSummary(false);

  }

  addNewUserTask() {

    let route = '/userTask/add';
    this.router.navigate([route], { state: { returnUrl: '/prospectives/true' } });
  }

  statusPendingClicked() {
    if(this.sParams.status==='Pending') return;
    this.sParams.status="Pending";
    this.service.setSummaryParams(this.sParams);
    //this.statusParamsRefresh();
  }

  statusConcludedClicked() {
    if(this.sParams.status==='Concluded') return;
    this.sParams.status="Concluded";
    this.service.setSummaryParams(this.sParams);
    //this.statusParamsRefresh();
  }

  statusAllClicked() {
    if(this.sParams.status==='') return;
    this.sParams.status="";
    this.service.setSummaryParams(this.sParams);
    //this.statusParamsRefresh();
  }

  statusParamsRefresh(){
    var xParams = this.service.getSummaryParams();
    xParams.status = this.sParams.status;
    this.service.setSummaryParams(xParams);

    this.getSummary();
  }

}
