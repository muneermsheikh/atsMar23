import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { IEmploymentDto } from 'src/app/shared/dtos/admin/employmentDto';
import { ISelPendingDto } from 'src/app/shared/dtos/admin/selPendingDto';
import { ISelectionStatus } from 'src/app/shared/models/admin/selectionStatus';
import { SelDecisionParams } from 'src/app/shared/params/admin/selDecisionParams';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { SelectionService } from '../selection.service';
import { IPagination } from 'src/app/shared/models/pagination';
import { CreateSelDecision } from 'src/app/shared/models/admin/createSelDecision';
import { selDecisionsToAddParams } from 'src/app/shared/params/admin/selDecisionsToAddParams';
import { ISelectionDecision } from 'src/app/shared/models/admin/selectionDecision';

@Component({
  selector: 'app-selection',
  templateUrl: './selection.component.html',
  styleUrls: ['./selection.component.css']
})
export class SelectionComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  selection: ISelPendingDto| undefined;
  selectionsPending: ISelPendingDto[]=[];
  selectionStatus: ISelectionStatus[]=[];
  employmentsDto: IEmploymentDto[]=[];
  
  sParams = new SelDecisionParams();
  totalCount = 0;

  cvsSelected: ISelPendingDto[]=[];

  pageIndex=1;
    
  todayDate = new Date(Date.now());
  statusSelected=10;

  loading=false;

  form: FormGroup = new FormGroup({});
  bsModalRef?: BsModalRef;

  //boolean 
  SelMsgsToCandidates=false;
  SelSMSToCandidates=false;
  RejMsgsToCandidates=false;
  RejSMSToCandidates=false;
  MsgsToClient=false;

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By Order No', value:'orderno'},
    {name:'By Order No Desc', value:'ordernodesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Agent', value:'agent'},
    {name:'By Agent Desc', value:'agentdesc'}
  ]

  constructor(private service: SelectionService, 
      private confirmService: ConfirmService,
      private toastr: ToastrService, 
      private router: Router,
      private activatedRoute: ActivatedRoute,
      private bsModalService: BsModalService) { }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe(data => { 
      this.selectionsPending = data.selectionsPending.data;
      this.selectionStatus = data.selectionStatus;
      this.totalCount = data.selectionsPending.count;
      //console.log('response:', data);
      
      if (this.selectionsPending.length === 0) this.toastr.warning('no pending selections');

      //set checkbox default to false
      /* const checkbox = document.getElementById('msgToClient') as HTMLInputElement;
      checkbox.checked = true;
      */
    })

  }

  getPendingSelections(useCache: boolean)
  {
    this.service.setParams(this.sParams);

    this.service.getPendingSelections(useCache).subscribe({
      next: response => {
        this.selectionsPending = response.data;
        this.totalCount = response.count;
        if (this.selectionsPending.length === 0) this.toastr.warning('no pending selections');
    
      },
      error: error => this.toastr.error(error)
    });
  }
    

  getSelectionStatus() {
    return this.service.getSelectionStatus().subscribe(response => {
      this.selectionStatus = response;
    }, error => {
      this.toastr.error(error);
    })
  }

  convertSelDecisionToDto (sel: ISelPendingDto[]): any {  //CreateSelDecision[] |undefined | null {
    console.log('sel',sel);
    if(sel.length===0) {
      this.toastr.warning('no selections made to save');
      return undefined;
    } 
    var dtos: CreateSelDecision[]=[];

    sel.forEach(s => {
      var dto = new CreateSelDecision();
      dto.cVRefId = s.cvRefId;
      dto.selectionStatusId = s.selectionStatusId;
      dto.decisionDate = this.todayDate;
      dto.remarks = s.remarks ?? '';
      //var emp = this.employmentsDto.find(x => x.cVRefId === s.cVRefId);
      //if (emp !== null) dto.employment = emp;
      dtos.push(dto);
    })
    console.log('dtos in function', dtos);
    return dtos;
  
  }

  registerSelections() {
    
    this.cvsSelected = this.selectionsPending.filter(x => x.checked===true);

    if (this.cvsSelected === null) {
      this.toastr.warning('no CVs selected');
      return;
    }

    //convert selDecision to dto
    var dtos =  this.convertSelDecisionToDto(this.cvsSelected);
    console.log('sel converted to dto:', dtos);
    var paramsToAdd = new selDecisionsToAddParams();
    paramsToAdd.selDecisionsToAddDto=dtos;
    console.log('after paramstadd.seldecisiontoadddto:', paramsToAdd);
    paramsToAdd.advisesToClients=this.MsgsToClient;
    paramsToAdd.rejectionEmaiLToCandidates=this.RejMsgsToCandidates;
    paramsToAdd.rejectionSMSToCandidates=this.RejSMSToCandidates;
    paramsToAdd.selectionEmailToCandidates=this.SelMsgsToCandidates;
    paramsToAdd.selectionSMSToCandidates=this.SelSMSToCandidates;
    
    console.log('paramsToAdd', paramsToAdd);

    return this.service.registerSelectionDecisions(paramsToAdd).subscribe(response => {
      this.toastr.success('selection decisions registered');
      console.log('rturned from api:', response);
      var affectedIds=response.cvRefIdsAffected;
      console.log('affecteIds', affectedIds, 'length:', affectedIds.length);
      if (affectedIds.length === 0) return;

      affectedIds.forEach(i =>{
          var index = this.selectionsPending.findIndex(x => x.cvRefId===i);
          if (index >=0) this.selectionsPending.splice(index,1);
        })
      }
    ), (error: any) => {
      this.toastr.error(error);
    }
    
  }

  editSelection(sel: ISelectionDecision) {
    return this.service.editSelectionDecision(sel).subscribe(response => {
      this.toastr.success('selection decision updated');
    }, error => {
      this.toastr.error(error);
    })
  }

  deleteSelection(id: number) {
    return this.service.deleteSelectionDecision(id).subscribe(response => {
      this.toastr.success('the chosen selection deleted');
    }, error => {
      this.toastr.error(error);
    })
  }

 /* showEmploymentModal(sel: ISelPendingDto){

    console.log('showemploymentmodal', sel);
    
      const title = 'Employment details: Application No.: ' + sel.applicationNo  +
        ', ' + sel.candidateName + '. Selected by: ' + sel.customerName +
        ', category ref: ' + sel.categoryName;
      var emp = this.getEmployment(sel);
      if (emp === undefined) {
        this.toastr.warning('failed to get emp');
        return;
      }
      const config = {
        class: 'modal-dialog-centered',
        initialState: {
          title,
          emp
        }
      }
      console.log('config', config);
      this.bsModalRef = this.bsModalService.show(SelectionModalComponent, config);
      this.bsModalRef.content.updateEmployment.subscribe(values => {
        this.service.updateEmployment(values).subscribe(() => {
            emp = values;
            this.toastr.success('employment data created');
        }, error => this.toastr.error(error))
      })
  }
  */

 /*
  getEmployment(sel: ISelPendingDto): IEmployment {
    var id = sel.cVRefId;
    var emp: IEmployment;

      this.service.getEmployment(id).subscribe(response => {
        emp = response;
      }, error => {
        this.toastr.error(error);
      })

    return emp;
  }
  */

  routeChange() {
    /* if (this.form.dirty) {
        this.confirmService.confirm('Confirm move to another page', 
        'This candidate selection decision form is edited, but not saved. ' + 
        'Do you want to move to another page without saving the data?')
        .subscribe(result => {
          if (result) {
            this.router.navigateByUrl('');
          }
        })
    } else {
      this.router.navigateByUrl('');
    } */
    this.router.navigateByUrl('/admin');
  }

  onPageChanged(event: any){
    const params = this.service.getParams();
    if (this.sParams.pageIndex !== event) {
      this.sParams.pageIndex = event;
      this.getPendingSelections(true);
    }
  }

  onSortSelected(event: any) {
    var sort = event.value;

    console.log('sort', sort);
    const prms = this.service.getParams();
    prms.sort = sort;
    prms.pageIndex=1;
    prms.pageSize=10;
    this.service.setParams(prms);
    this.getPendingSelections(true);
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageIndex = 1;
    this.service.setParams(params);
    this.getPendingSelections(true);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new SelDecisionParams();
    this.service.setParams(this.sParams);
    this.getPendingSelections(false);
  }
  


}
