import { Component, ElementRef, EventEmitter, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IDeploymentPendingDto } from 'src/app/shared/dtos/process/deploymentPendingDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { Deployment, IDeployment } from 'src/app/shared/models/process/deployment';
import { depProcessParams } from 'src/app/shared/params/process/depProcessParams';
import { deployParams } from 'src/app/shared/params/process/deployParams';
import { DeployService } from '../deploy.service';
import { DepModalComponent } from '../dep-modal/dep-modal.component';
import { BreadcrumbService } from 'xng-breadcrumb';
import { IDeployStage } from 'src/app/shared/models/masters/deployStage';
import { map, switchMap } from 'rxjs/operators';
import { DeployAddModalComponent } from '../deploy-add-modal/deploy-add-modal.component';
import { DeployEditComponent } from '../deploy-edit/deploy-edit.component';
import { ICVReferredDto } from 'src/app/shared/dtos/admin/cvReferredDto';


@Component({
  selector: 'app-deploy-list',
  templateUrl: './deploy-list.component.html',
  styleUrls: ['./deploy-list.component.css']
})
export class DeployListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  @Input() EditDepEvent = new EventEmitter();

  user?: IUser;
  returnUrl = '';

  referrals: IDeploymentPendingDto[]=[];
  editedReferrals: IDeploymentPendingDto[]=[];

  routeId: string='';
  pParams = new deployParams();
  totalCount: number=0;
  
  depStatuses: IDeployStage[]=[];

  bsModalRef: BsModalRef | undefined;

  currentCVRefId: number=0;

  dParams = new depProcessParams();
  
  searchOptions = [
      {name:'Application', value:'applicationno'},
      {name:'Candidate Name', value:'candidateName'},  
      {name:'Order No', value:'orderNo'},  
      {name:'Category Name', value:'categoryName'},  
      {name:'Date Selected', value:'selectedon'},  
      {name:'Employer Name', value:'customerName'}
  ]

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Employer', value:'employer'},
    {name:'By Employer Desc', value:'employerdesc'}
  ]

 
  constructor(
      private activatedRoute: ActivatedRoute
      , private toastr: ToastrService
      , private modalService: BsModalService
      , private service: DeployService
      , private router: Router
      , private bcService: BreadcrumbService
      ) 
    { 
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if( nav.extras.state.user) {
              this.user = nav.extras.state.user as IUser;
              //this.hasEditRole = this.user.roles.includes('AdminManager');
              //this.hasHRRole =this.user.roles.includes('HRSupervisor');
            }
            //if(nav.extras.state.user) this.user=nav.extras.state.user as IUser;
        }
        this.bcService.set('@deployentList',' ');
    }

  ngOnInit(): void {
    this.getProcesses(false);

    this.getDeploymentStatuses();
  }

  getDeploymentStatuses() {
    this.service.getDeployStatus().subscribe({
      next: response => this.depStatuses = response,
      error: error => console.log(error)
    });
  }


  onPageChanged(event: any){
    const params = this.service.getOParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setOParams(params);

      this.getProcesses(true);
    }
  }

  getProcesses(useCache = false) {
    
   this.service.getPendingProcessesPaginated(useCache)?.subscribe({
    next: response => {
      if(response !== undefined && response !== null) {
        this.referrals = response.data;
        this.totalCount = response?.count;
      } else {
        console.log('response is undefined');
      }
    },
    error: error => console.log(error)
   })

  }

  updateDeployments() {
    if(this.editedReferrals.length===0) {
      this.toastr.warning('No records changed to update');
      return;
    }

    var newDeploys = this.ConverDtoToDeploys();

    this.service.InsertDeployTransactions(newDeploys).subscribe((response : any) => {
      if(response) {
        this.toastr.success('all deplyments successfully added');
        newDeploys.forEach(x => {

        })
      } else {
        this.toastr.warning('failed to add the deployment transactions');
      }
     }, error => {
        this.toastr.error('failed to add the transactions: ' + error);
      }
    )
  
  }

  ConverDtoToDeploys(): IDeployment[] {
    var newDeploys:IDeployment[]=[];

    this.editedReferrals.forEach(dto => {
      var dep = new Deployment();
      dep.id=dto.id;
      dep.deployCVRefId=dto.deployCVRefId;
      dep.sequence=dto.deploySequence;
      dep.transactionDate=dto.deployStageDate;
      //dep.nextStageId=dep.nextStageId;
      //dep.nextEstimatedStageDate=dep.nextEstimatedStageDate;
      newDeploys.push(dep);
    })

    return newDeploys;
    
  }

  close() {
    this.currentCVRefId=0;    //closes the app-deployments selector
  }

  /*displayModalDeployment(indx: number)
  {
    var record = this.referrals.filter(x => x.id==indx)[0];
    var ref = this.referrals.filter(x => x.id==indx).map(x => x.id)[0];
    this.service.getCVRefDeployments(ref).subscribe((response : any) => {
      if (response===undefined) {
        console.log('failed to get CVReferredDto from API');
        return;
      }
      //this.cvDto = response;
    
      const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          cvRefId : record.id,
          candidatename: record.candidateName,
          companyName: record.customerName,
          applicationNo: record.applicationNo,
          categoryRef: record.orderNo + "-" + record.categoryName,
          selectedAs: record.categoryName,
          depStatuses : this.depStatuses
        }
      };

      this.bsModalRef = this.modalService.show(DepModalComponent, config);
  
      this.bsModalRef.content.emitObj.subscribe((values: any) => {
        this.toastr.success('success');
  
      }, (error : any) => {
        console.log('error modal', error);
      })
  

    }, error => {
      console.log('failed to get CVReferredDto', error);
      return;
    })

  }
*/

  onSearchSelected(searchParam: any) {
    var searchParams = searchParam.value;
    const params = this.service.getOParams();
    params.searchOn = searchParams;
    //params.pageNumber = 1;
    this.service.setOParams(params);
    
  }

  
  onSearch() {
    const params = this.service.getOParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setOParams(params);
    this.getProcesses();
  }


  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.pParams = new deployParams();
    this.service.setOParams(this.pParams);
    this.getProcesses();
  }
  
  onSortSelected(sortSt: any) {
    var sort = sortSt.value;
    const prms = this.service.getOParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.service.setOParams(prms);
    this.getProcesses();
  }

  editDeployment(pendingDto: IDeploymentPendingDto)
  {
      this.navigateByRoute(pendingDto.deployCVRefId, '/processing/edit', true, this.depStatuses);
  }

  addNewDeployment(dto: IDeploymentPendingDto)
  {

    this.service.getCVReferredDto(dto.deployCVRefId).subscribe({
      next: (ref: ICVReferredDto) => {
        console.log('ref:', ref);
        if(ref===null) {
          this.toastr.info('Failed to retrieve deployent data from API');
          return;
        }

          const config = {
            class:'modal-dialog-centered modal-lg',
            initialState: {
              ref,
              deployStatuses: this.depStatuses
            }
          };
    
          this.bsModalRef = this.modalService.show(DeployAddModalComponent, config);
          
          this.bsModalRef.content.EditEvent.subscribe({
            next: (editedDep: boolean) => {
                  if(editedDep === true) {
                    this.toastr.success('deployment record updated');
                  } else {
                    this.toastr.warning('Edit aborted');
                  }},
            error: (err: any) => this.toastr.error('error in updating deployment record')
            
        }); 
    
    /*
      this.bsModalRef.content.EditEvent.pipe(
        switchMap((values: ICVReferredDto) => this.service.updateDeployment(values).pipe(
          
          catchError(err => {
            console.log('Error in updating Employment Record', err);
            return of();
          }),
          tap(res => console.log('Deployment Record sent to and recd back from api: ', values, res)),
        )),
        catchError(err => {
          console.log(err);
          this.toastr.error('Error in getting updated object from Employment Edit Modal', err);
          return of();
        })
      ).subscribe( () => {
        this.toastr.success('deployment record updated');
        console.log()
      }),
      (err: any) => {
        console.log('unhandled error NOT handled in catch Error(, or if throwError()')
      }
      */

    },
    error: err => this.toastr.error('error:', err)
    })
  }

  showTransactions(cvrefid: number) {
    this.service.getCVReferredDto(cvrefid).subscribe(dto => {
      const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          cv: dto,
          deployStatuses: this.depStatuses,
        }
      };

      this.bsModalRef = this.modalService.show(DeployEditComponent, config);

      this.bsModalRef.content.EditDepEvent.subscribe({
        next: (deploys: IDeployment[]) => {
          this.service.updateDeployment(deploys).subscribe((uccess:boolean) => {
            this.toastr.success('deployments updated');
          })
        },
        error: (err: any) => {
          this.toastr.info('failed to update the deployments')
        }
      });
  })
}


  editDep(pendingDto: IDeploymentPendingDto) {

    this.service.getDeployments(pendingDto.deployCVRefId).subscribe(dto => {
      const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          dep: dto,
          depStatuses: this.depStatuses,
  
          cvrefid : pendingDto.deployCVRefId,
          candidatename: pendingDto.candidateName,
          companyName: pendingDto.customerName,
          applicationNo: pendingDto.applicationNo,
          //referredOn: dto.referredOn,
          categoryRef: pendingDto.orderNo + "-" + pendingDto.categoryName,
          selectedAs: pendingDto.categoryName,
        }
      };
  
      this.bsModalRef = this.modalService.show(DepModalComponent, config);
      
      this.bsModalRef.content.emitObj.pipe(
        
        switchMap((values: IDeployment[]) => {
          console.log('updating api with values:', values);
          return this.service.updateDeployment(values).pipe(
            map(res => {
              if(!res) this.toastr.warning('failed to update the deployments data');
              return false;
            })
          )
        })
      )
    })
      

    

  }

  editDepTransactions(dep: any)
  {
    //this.navigateByRoute(dep.CVRefId, '/processing/edit', true, this.depStatuses);
  }


  navigateByRoute(id: number, routeString: string, editable: boolean, obj: any[]) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/processing/list',
            depStages: obj
          } }
      );
  }

  getNextSeqForNextTransaction(seq: number) {
    var nextSeq = this.depStatuses.find(x => x.sequence == seq)?.nextSequence ?? 0;
    console.log('Next Sequence for next Transaction:', nextSeq);
    return nextSeq;    
  }

  getSequenceForNextTransaction(dep: IDeploymentPendingDto): number {

    var currentSeq = this.findMaxSeq(dep);

    var seqForNextTrasaction = this.depStatuses.find(x => x.sequence == currentSeq)?.nextSequence;
    console.log('next sequence:', seqForNextTrasaction);
    return seqForNextTrasaction ?? 0;    
  }
  
  findMaxSeq(dep: IDeploymentPendingDto){
    //first row has the max seq, by design/.;
    var t= dep.deploySequence;
    
    return t;
  }

  getNextStageDateForNextTransaction(dep: IDeploymentPendingDto, nextSeq: number) {
    var lastDt: Date = new Date(dep.deployStageDate);
    
    var days = this.depStatuses.find(x => x.sequence==  nextSeq)?.estimatedDaysToCompleteThisStage;
    
    return new Date(lastDt.setDate(lastDt.getDate() + days!));

  }

}