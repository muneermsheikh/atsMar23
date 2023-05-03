import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';
import { ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { ICustomerNameAndCity } from 'src/app/shared/models/admin/customernameandcity';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICandidateCity } from 'src/app/shared/models/hr/candidateCity';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { paramsCandidate } from 'src/app/shared/params/hr/paramsCandidate';
import { CandidateService } from '../candidate.service';
import { OrderService } from 'src/app/orders/order.service';
import { UploadDownloadService } from '../upload-download.service';
import { IdsModalComponent } from 'src/app/orders/ids-modal/ids-modal.component';
import { IOrderItemBriefDto } from 'src/app/shared/dtos/admin/orderItemBriefDto';

@Component({
  selector: 'app-candidates-listing',
  templateUrl: './candidates-listing.component.html',
  styleUrls: ['./candidates-listing.component.css']
})
export class CandidatesListingComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  
  user: IUser | undefined;
  cvs: ICandidateBriefDto[]=[];
  selectedCVs: ICandidateBriefDto[]=[];

  cvParams = new paramsCandidate();
  totalCount: number=0;
  candidateCities: ICandidateCity[]=[];
  professions: IProfession[]=[];
  existingQBankCategories: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  bsModalRef: BsModalRef | undefined;

  idFromChild: number=0;

//ngSelect
  selectedProfIds: number[]=[];
  events: Event[] = [];

  documentLoading=false;

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By City Asc', value:'city'},
    {name:'By City Desc', value:'citydesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Agent', value:'agent'},
    {name:'By Agent Desc', value:'agentdesc'}
  ]

  constructor(
      private service: CandidateService, 
      private accountService: AccountService,
      private router: Router,
      private activatedRoute: ActivatedRoute, 
      private modalService: BsModalService,
      private orderService: OrderService, 
      private downloadservice: UploadDownloadService,
      private toastr: ToastrService) { 

    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    
      }

  ngOnInit(): void {
    this.getCVs(false);
    
    this.activatedRoute.data.subscribe(data => {
        //this.cvs = data.candidateBriefs,
        //this.totalCount = data.candidateBriefs.count,
        this.professions = data.professions,
        this.agents = data.agents
      }

      
    )
    this.getCities();
    //this.getProfessions();
    //this.getAgents();
    //console.log('CVs list', this.cvs);
  }
  
  getCVs(useCache=false) {
    this.service.getCandidates(useCache).subscribe({
      next: response => {
        this.cvs = response.data;
        this.totalCount = response.count;
      },
      error: error => this.toastr.error(error)
    });
  }

  getCities() {
    this.service.getCandidateCities().subscribe(response => {
      this.candidateCities = [{city: 'All'}, ...response];
    })
  }

  /* getProfessions() {
    this.mastersService.getCategoryList().subscribe({
      next: response => {
        this.professions=response;
        console.log('categories:', this.professions);
      },
      error: error => console.log(error)
    })
  }
  
  getAgents() {
    this.mastersService.getAgents().subscribe(response => {
      this.professions = [{id: 99999999, name: 'All'}, ...response];
      //this.agents = [...response];
    }, error => {
      console.log(error);
    })
  }
  */
  onSearch() {
    const params = this.service.getCVParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setCVParams(params);
    this.getCVs();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cvParams = new paramsCandidate();
    this.service.setCVParams(this.cvParams);
    this.getCVs();
  }
  
  onSortSelected(sort: any) {
    console.log('sort', sort);
    const prms = this.service.getCVParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.service.setCVParams(prms);
    this.getCVs();
  }

  onCitySelected(citySelected: any) {
    console.log('citySelected', citySelected);
    const prms = this.service.getCVParams();
    prms.city = citySelected;
    prms.pageNumber=1;
    this.service.setCVParams(prms);
    this.getCVs();
  }

  onProfSelected(profId: number) {
    console.log('profid', profId);
    const prms = this.service.getCVParams();
    prms.professionId = profId;
    prms.pageNumber=1;
    this.service.setCVParams(prms);
    this.getCVs();

  }
  
  onAgentSelected(agentId: any) {
    console.log('agentId', agentId);
    const prms = this.service.getCVParams();
    prms.agentId = agentId;
    prms.pageNumber=1;
    this.service.setCVParams(prms);
    this.getCVs();

    //console.log('after profession selected', this.cvs);

  }

  onCategorySelected(profId: any) {
    const prms = this.service.getCVParams();
    prms.professionId = profId;
    prms.pageNumber=1;
    this.service.setCVParams(prms);
    this.getCVs();

    //console.log('after profession selected', this.cvs);

  }

  onPageChanged(event: any){
    const params = this.service.getCVParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setCVParams(params);
      this.getCVs(true);
    }
  }

  //Having selected candidates, refer them to internal reviews or directly to client
  openChecklistModal(user: IUser) {
    const title = 'Choose Order Item to refer selected CVs to';
    var returnvalue:any;
    var ids: number[]=[];
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        title,
        orderItems: this.getOpenOrderItemsArray(),
        ids
      }
    }
    this.bsModalRef = this.modalService.show(IdsModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((values: number[]) => {
      ids = values;
      if (ids.length) {
        //this.service.submitCVsForReview().subscribe(() => {
//          user.roles = [...rolesToUpdate.roles]
        //}
        //)
      }
    })
  }
  

  private getOpenOrderItemsArray(): IOrderItemBriefDto[] {
    const roles: any[] = [];
    let aitems: IOrderItemBriefDto[]=[];
    let aitem: IOrderItemBriefDto;
    this.orderService.getOrderItemsBriefDto().subscribe(response => {
      aitems = response;
      if(aitems.length===0) return;
      return aitems;
    }, error => {
      console.log('failed to retrieve roles array', error);
    })
    return aitems;
  }

  //output value from child
  showDocumentViewerEvent(id: number){
    this.idFromChild = id;
    return this.service.viewDocument(id).subscribe(result => {
      this.loadInitialDocument(result);
    }, error => {
      this.toastr.error(error);
    })
    
  }

  downloadFileEvent(candidateid: number) {
    return this.downloadservice.downloadFile(candidateid).subscribe(response => {
      this.toastr.success('document downloaded');
    }, error => {
      this.toastr.error('failed to download document', error);
    })
  }

  cvAssessEvent(cvbrief: ICandidateBriefDto)
  {
    this.navigateByUrl('/hr/cvassess/' + cvbrief.id, undefined, false);
  }

  navigateByUrl(route: string, cvObject: any|undefined, toedit: boolean) {
    this.router.navigate(
      [route],
      { state: 
        { 
          cvbrief: cvObject, 
          //openorderitems,
          //assessmentsDto,
          userobject: this.user,
          toedit: toedit, 
          returnUrl: '/candidates' 
        } }
    )
  }

  cvCheckedEvent(cvbrief: ICandidateBriefDto) {
      var cv = this.selectedCVs.find(x => x.id==cvbrief.id);
      var index = this.selectedCVs.findIndex(x => x.id===cvbrief.id);

      if(cv !==undefined) {
        if(!cv.checked) {
          this.selectedCVs.splice(index,1);
        } else {
          this.selectedCVs.push(cvbrief);  
        }
      } else {
        this.selectedCVs.push(cvbrief);
      }

  }

  cvEditClicked(id: number) {

    this.navigateByUrl('/candidates/edit/' + id, undefined, true);
  }

  cvAssessClicked() {
    this.navigateByUrl('/hr/assessments', this.selectedCVs, false);
  }


  loadInitialDocument(document: any) {
      
  }

  
  
  
}
