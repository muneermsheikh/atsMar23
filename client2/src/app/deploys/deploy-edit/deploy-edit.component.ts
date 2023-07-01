import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ICVReferredDto, IDeployDto } from 'src/app/shared/dtos/admin/cvReferredDto';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ToastrService } from 'ngx-toastr';
import { DeployService } from '../deploy.service';
import { IDeployStage } from 'src/app/shared/models/masters/deployStage';


@Component({
  selector: 'app-deploy-edit',
  templateUrl: './deploy-edit.component.html',
  styleUrls: ['./deploy-edit.component.css']
})
export class DeployEditComponent implements OnInit {

  cv: ICVReferredDto | undefined;
  bsModalRef: BsModalRef | undefined;
  deployStatuses: IDeployStage[]=[];
  
  constructor(private activatedRoute: ActivatedRoute
    , private router: Router
    , private bcService: BreadcrumbService
    , private modalService: BsModalService
    , private toastr: ToastrService
    , private service: DeployService) 
    {
        /*
        let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if( nav.extras.state.user) {
              this.user = nav.extras.state.user as IUser;
              //this.hasEditRole = this.user.roles.includes('AdminManager');
              //this.hasHRRole =this.user.roles.includes('HRSupervisor');
            }
            if( nav.extras.state.depStages) {
              this.deployStatuses = nav.extras.state.depStages as IDeployStage[];
            }
            if(nav.extras.state.user) this.user=nav.extras.state.user as IUser;
        }
        this.bcService.set('@editDeployment',' ');
        */
   }


  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => {
      this.cv = data.deployment
    })

    console.log('epStatus:', this.deployStatuses);
  }

  addTransaction() {
    this.service.getCachedProcessesList(false);

    var record = this.cv!;
    
    var iStageId = this.getNextStatusId();
    var iNextStageId=this.getNextStatusIdFromDepSatus(iStageId!);
    var iNextEstimatedStageDate: Date = this.getNextStatusDateEstimated(iNextStageId!);
    
    const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          cvRefId : record.cvRefId,
          transactionDate: new Date(),
          stageId:iStageId,
          nextStageId: iNextStageId,
          nextEstimatedStageDate: iNextEstimatedStageDate,
          candidateName: record.candidateName,
          companyName: record.customerName,
          referredOn: record.referredOn,
          applicationNo: record.applicationNo,
          categoryRef: record.orderNo + "-" + record.categoryName,
          selectedAs: record.categoryName,
          deployStatuses: this.deployStatuses
        }
      };

    }


    getNextStatusId() {
      
      var nextStatusId = this.getNextStatusIdFromDepSatus(this.getLastStatusId()!); 
      console.log('getNextStatusId:', nextStatusId);
      return nextStatusId;
    }

    getLastStatusId() {
      var lastId = this.cv?.deployments[0].sequence; //deployments is sorted in desc order by dates
      console.log('lastid:', lastId);
      return lastId;
    }

    getNextStatusIdFromDepSatus(statusId: number) {
      var nextSeq = this.deployStatuses.find(x => x.id == statusId)?.nextSequence;
      var nextId = this.deployStatuses.find(x => x.sequence===nextSeq)?.id;
      return nextId;
    }

    getNextStatusDateEstimated(nextStatusId: number): Date {

      var dt = this.cv?.deployments.find(x => x.sequence == nextStatusId)?.
        transactionDate;

      var days = this.deployStatuses.find(x => x.id == nextStatusId)?.estimatedDaysToCompleteThisStage;
      
      var temp = dt?.setDate(dt.getDate() + days!);
      if(!Number.isNaN(temp)) {
        if(temp !== undefined) return new Date(temp);
      
      } 
      
      return new Date();
      
    }

    editTransaction(dep: IDeployDto)
    {

    }

    deleteTransaction(dep: IDeployDto)
    {

    }

    /*
    returnToBack() {
      this.router.navigateByUrl(this.returnUrl);
    }
    */

}
