import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ChecklistService } from 'src/app/hr/checklist.service';
import { CandidateAssessedDto, ICandidateAssessedDto } from 'src/app/shared/dtos/hr/candidateAssessedDto';
import { ICustomerNameAndCity } from 'src/app/shared/models/admin/customernameandcity';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { IPagination } from 'src/app/shared/models/pagination';
import { CVRefParams } from 'src/app/shared/params/admin/cvRefParams';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { CvrefService } from '../cvref.service';
import { IChecklistHRDto } from 'src/app/shared/dtos/hr/checklistHRDto';
import { ChecklistModalComponent } from 'src/app/candidates/checklist-modal/checklist-modal.component';
import { ICheckedAndBoolean } from 'src/app/shared/dtos/admin/checkedAndBooean';


@Component({
  selector: 'app-cvref',
  templateUrl: './cvref.component.html',
  styleUrls: ['./cvref.component.css']
})
export class CvrefComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;

  cvsAssessed: ICandidateAssessedDto[]=[];
  form: FormGroup = new FormGroup({});
  cvsAssessedSelected: ICandidateAssessedDto[]=[];
  assessmentids: number[]=[];
  bsModalRef?: BsModalRef;
  
  pagination?: IPagination<ICandidateAssessedDto[]>;  //ShortlistedCVs;

  cvRefParams = new CVRefParams();
  totalCount: number=0;
  agents: ICustomerNameAndCity[]=[];
  professions: IProfession[]=[];

  sortOptions = [
    {name:'By Applicn No Asc', value:'appno'},
    {name:'By Applicn No Desc', value:'apppnodesc'},
    {name:'By Name Asc', value:'name'},
    {name:'By Name Desc', value:'namedesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Agent', value:'agent'},
    {name:'By Agent Desc', value:'agentdesc'}
  ]

  constructor(private cvrefService: CvrefService, 
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private checklistService: ChecklistService,
    private confirmService: ConfirmService,
    private bsModalService: BsModalService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.cvrefService.setCVRefParams(this.cvRefParams);
    this.getShortlistedCVs(false);

    this.activatedRoute.data.subscribe(data => { 
      //this.cvAssessed = data.assessedcvs;
      this.professions = data.professions;
      this.agents = data.agents;
    })
    //console.log('agents:', this.agents, 'professions:', this.professions);
  }

  getShortlistedCVs(useCache=false) {
    this.cvrefService.setCVRefParams(this.cvRefParams);
    
    this.cvrefService.getShortlistedCandidates(useCache).subscribe(response => {
      if(response===null) {
        this.toastr.info('No shortlisted candidates exist');
        return;
      } else {
        this.cvsAssessed = response.data;
        this.totalCount = response.count;
      }
      console.log('cvassessed', this.cvsAssessed);
    }, (error: any) => {
      console.log(error);
    })
  }
 
  updatecvsAssessedSelected(dto: CandidateAssessedDto) {

    var index = this.cvsAssessedSelected.findIndex(x => x.id==dto.id);

    if (dto.checked) {
        if (index===null) {  
            var newItem = new CandidateAssessedDto();
            newItem.checked=true;
            newItem.id=dto.id;
            this.cvsAssessedSelected.push(newItem);
        } else {
          this.cvsAssessedSelected[index].checked=dto.checked;
        }
    } else {
        if(index!==null) {
          this.cvsAssessedSelected.splice(index,1);
        }
    }
  }

  cvChecked(obj: ICheckedAndBoolean)
  {
    console.log('obj:', obj);
    var index = this.cvsAssessedSelected.findIndex(x => x.id==obj.id);

    if (obj.checked) {
      console.log('obj checked');
        if (index===-1) {  
          console.log('checked, and add new');
            var newItem = new CandidateAssessedDto();
            newItem.checked=true;
            newItem.id=obj.id;
            this.cvsAssessedSelected.push(newItem);
        } else {
          console.log('obj not checked');
          this.cvsAssessedSelected[index].checked=obj.checked;
          this.cvsAssessedSelected.splice(index,1);
        }
    } else {
      console.log('spliced, index=', index, this.cvsAssessedSelected);
        if(index!==-1) {
          this.cvsAssessedSelected.splice(index,1);
        }
    }
    
    console.log(this.cvsAssessedSelected);
  }

  onSearch() {
    const params = this.cvrefService.getCVRefParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.cvrefService.setCVRefParams(params);
    this.getShortlistedCVs();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cvRefParams = new CVRefParams();
    this.cvrefService.setCVRefParams(this.cvRefParams);
    this.getShortlistedCVs();
  }
  
  onSortSelected(sortString: any) {
    var sort = sortString;
    const prms = this.cvrefService.getCVRefParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.cvrefService.setCVRefParams(prms);
    this.getShortlistedCVs();
  }

  onProfSelected(profId: number) {
    const prms = this.cvrefService.getCVRefParams();
    prms.professionId = profId;
    prms.pageNumber=1;
    this.cvrefService.setCVRefParams(prms);
    this.getShortlistedCVs();

  }
  
  onAgentSelected(agentId: number) {
    console.log('agentId', agentId);
    const prms = this.cvrefService.getCVRefParams();
    prms.agentId = agentId;
    prms.pageNumber=1;
    this.cvrefService.setCVRefParams(prms);
    this.getShortlistedCVs();
  }

  onCategorySelected(profId: number) {
    const prms = this.cvrefService.getCVRefParams();
    prms.professionId = profId;
    prms.pageNumber=1;
    this.cvrefService.setCVRefParams(prms);
    this.getShortlistedCVs();

    //console.log('after profession selected', this.cvs);

  }

  onPageChanged(event: any){
    const params = this.cvrefService.getCVRefParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.cvrefService.setCVRefParams(params);
      this.getShortlistedCVs(true);
    }
  }

  goBack() {
      this.router.navigateByUrl('/admin');
  }

  forwardSelected() {
    this.assessmentids = this.cvsAssessed.filter(x => x.checked===true).map(x => x.id);
    
    this.cvrefService.referCVs(this.assessmentids).subscribe(response => {
      console.log('response from api;', response);
      if (response.errorString==='' || response.errorString===null) {
          this.toastr.success('CVs referred and email messages composed');

          //delete records tha are forwarded from the pending list, i.e. 
          var assessmentIdsForwarded = response.cvRefIds;
          assessmentIdsForwarded.forEach(id => {
            let indx = this.cvsAssessed.findIndex( element => element.id===id);
              console.log('idsforwarded:', assessmentIdsForwarded, 'index=', indx);

              if (indx >=0) {
                console.log('spliced indx', indx);
                this.cvsAssessed.splice(indx,1); }
            });
          this.totalCount-=assessmentIdsForwarded.length;  
      } else {
        console.log('failed to forward cvs:', response.errorString);
        this.toastr.warning('failed to forward CVs -- ', response.errorString);
      }
      
    }, error => {
      
      this.toastr.error('failed to refer selected CVs', error);
    })
  }

  /*
  onCheckedChange($event: any) {
    console.log('event', $event);
    var index = this.cvsAssessed.findIndex(x => x.id === $event.id);
    if ($event.checked) {
      if (index === null) {
        this.cvsAssessedSelected.push($event);
      } else {
        var obj = this.cvsAssessed.find(x => x.id === $event.id);
        obj!.checked=true;
      }
    } else {
      this.cvsAssessedSelected.splice(index,1);
    }
  }
  */
   
  routeChange() {
    if (this.form.dirty) {
        this.confirmService.confirm('Confirm move to another page', 
        'This candidate assessment data is edited, but not saved. ' + 
        'Do you want to move to another page without saving the data?')
        .subscribe(result => {
          if (result) {
            this.router.navigateByUrl('');
          }
        })
    } else {
      this.router.navigateByUrl('');
    }
  }

  openChecklistModal(dto: ICandidateAssessedDto) {
    //let clist: IChecklistHRDto;
    var candidateid = dto.candidateId;
    var orderitemid = dto.orderItemId;

    this.checklistService.getChecklist(candidateid, orderitemid).subscribe(chklst => {
      const config = { class: 'modal-dialog-centered modal-lg', initialState: {chklst:chklst} }
      this.bsModalRef = this.bsModalService.show(ChecklistModalComponent, config);
      this.bsModalRef.content.updateChecklist.subscribe((values: any) => {
          this.checklistService.updateChecklist(values).subscribe(() => {
            this.toastr.success('checklist updated');
          }, error => {
            this.toastr.error('failed to update the checklist Service', error);
          })
      })  
    }, error => {
      console.log('error', error);
    })
  }

  private getChecklistHRDto(candidateid: number, orderitemid: number): any {
    let lst: IChecklistHRDto;
    return this.checklistService.getChecklist(candidateid, orderitemid).subscribe(response => {
      if (response === null) {
        this.toastr.warning('checklist record does not exist for the candidate for the selected order item id');
        return null;
      } else {
        lst = response;
        return lst;
      }
    }, error => {
      console.log('failed to return checklsit', error);
    })
  }

  //ngClass for charges
  


}
