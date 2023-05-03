import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import { ICandidateBriefDto } from 'src/app/shared/dtos/admin/candidateBriefDto';
import { IOrderItemBriefDto } from 'src/app/shared/dtos/admin/orderItemBriefDto';
import { CandidateAssessedDto, ICandidateAssessedDto } from 'src/app/shared/dtos/hr/candidateAssessedDto';
import { IChecklistHRDto } from 'src/app/shared/dtos/hr/checklistHRDto';
import { IHelp } from 'src/app/shared/models/admin/help';
import { IUser } from 'src/app/shared/models/admin/user';
import { CandidateAssessment, ICandidateAssessment } from 'src/app/shared/models/hr/candidateAssessment';
import { ICandidateAssessmentAndChecklist } from 'src/app/shared/models/hr/candidateAssessmentAndChecklist';
import { IChecklistHRItem } from 'src/app/shared/models/hr/checklistHRItem';
import { CvAssessService } from '../cv-assess.service';
import { ChecklistService } from '../checklist.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { AccountService } from 'src/app/account/account.service';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { take } from 'rxjs/operators';
import { CandidateAssessmentItem, ICandidateAssessmentItem } from 'src/app/shared/models/hr/candidateAssessmentItem';
import { IOrderItemAssessmentQ } from 'src/app/shared/models/admin/orderItemAssessmentQ';
import { HelpModalComponent } from 'src/app/shared/components/help-modal/help-modal.component';
import { ChecklistModalComponent } from 'src/app/candidates/checklist-modal/checklist-modal.component';

@Component({
  selector: 'app-cv-assess',
  templateUrl: './cv-assess.component.html',
  styleUrls: ['./cv-assess.component.css']
})
export class CvAssessComponent implements OnInit {

  cvBrief: ICandidateBriefDto | undefined;
  openOrderItems: IOrderItemBriefDto[]=[];
  orderItemSelected: IOrderItemBriefDto | undefined;
  orderItemSelectedId: number=0;
  cvAssessment: ICandidateAssessment | undefined;   //full assessment of the candidate

  helpfile = this.getHelp("candidate assessment");
  //checklist$: Observable<IChecklistHRDto>;

  assessmentAndChecklist: ICandidateAssessmentAndChecklist | undefined;
  existingAssessmentsDto: ICandidateAssessedDto[]=[];    //summary of existing assessments of the selected candidate

//checklist
  checklist: IChecklistHRDto | undefined;
  checklistitems: IChecklistHRItem[]=[];
  bsModalRef: BsModalRef | undefined;

  user: IUser | undefined;
  totalPoints: number=0;
  totalGained: number=0;
  percentage: number=0;
  qDesigned: boolean = false;
  requireInternalReview: boolean=false;
  lastOrderItemIdSelected: number=-1;

  routeId: string='';
  bolNavigationExtras: boolean=false;
  returnUrl: string='';

  form: FormGroup = new FormGroup({});
  validationErrors: string[] = [];

  bDisplayHelp:boolean=false;
  displayText: string='Show Help';

  //emit to child
  orderItemChangedEventSubject: Subject<ICandidateAssessment> = new Subject<ICandidateAssessment>();


  @HostListener('window:beforeunload', ['event']) unloadNotification($event: any) {
    if(this.form.dirty) {$event.returnValue=true}
  }

  constructor(private fb: FormBuilder, 
    private service: CvAssessService,
    private bsModalService: BsModalService,
    private checklistService: ChecklistService,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountService, 
    private confirmService: ConfirmService,
    private router: Router, private bcService: BreadcrumbService) {
      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
            
            this.routeId = this.activatedRoute.snapshot.params['id'];
            if(this.routeId==undefined) this.routeId='';

            let nav: Navigation | null  = this.router.getCurrentNavigation() ;
            
            if (nav?.extras && nav.extras.state) {
                this.bolNavigationExtras=true;
                if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

                if( nav.extras.state.cvbrief) this.cvBrief = nav.extras.state.cvbrief as ICandidateBriefDto;
                //if(nav.extras.state.openorderitems) this.existingAssessmentsDto = nav.extras.state.openorderitems as IAssessmentsOfACandidateIdDto[]; 
                //if(nav.extras.state.openorderitems) this.openOrderItems = nav.extras.state.openorderitems as IOrderItemBriefDto[];
            }
            this.bcService.set('@CVAssess',' ');
     }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.openOrderItems = data.openOrderItemsBrief;
      this.existingAssessmentsDto = data.assessmentsDto;
      console.log('existingAssessmentsDto',this.existingAssessmentsDto);
    })
    
   console.log('cvassessment.ts, existingAssessmentsDto', this.existingAssessmentsDto, 'openorderitems:', this.openOrderItems);
  }

  initializeTotals() {
    this.totalGained =0;
    this.totalPoints=0;
    this.percentage=0;

  }

  chooseSelectedOrderItem() {
    if (this.orderItemSelectedId <= 0 || this.orderItemSelectedId === undefined) return;

    if (this.lastOrderItemIdSelected === this.orderItemSelectedId) return;
    this.lastOrderItemIdSelected = this.orderItemSelectedId;

    this.initializeTotals();

    this.orderItemSelected = this.openOrderItems.find(x => x.orderItemId===this.orderItemSelectedId);
    this.requireInternalReview = this.orderItemSelected!.requireInternalReview??false;
    
    if(this.requireInternalReview) {
        this.qDesigned = this.orderItemSelected!.assessmentQDesigned;

        if (!this.qDesigned) {
          this.toastr.warning('assessment Questions not designed - from component');
          //this.candidateAssessmentItems.clear();
          return;
        }
    }
    
    this.service.getCVAssessmentAndChecklist(this.cvBrief!.id, this.orderItemSelected!.orderItemId).subscribe(response => {
      this.assessmentAndChecklist = response;
      
      //this.checklist = this.assessmentAndChecklist.checklistHRDto;
      this.checklist = response.checklistHRDto;
      this.cvAssessment = this.assessmentAndChecklist.assessed;

      if (this.cvAssessment !== null && this.cvAssessment !== undefined) {
        this.orderItemChangedEventSubject.next(this.cvAssessment);
      } else {
        this.toastr.warning('the candidate has not been assessed for the category selected.');
      }
    }, error => {
      this.toastr.error('failed to retrieve candidate assessment', error);
      //this.candidateAssessmentItems.clear();
    })

    
  }

  returnToCalling() {
    this.router.navigateByUrl(this.returnUrl || '' );
  } 

  createNewAssessment(){
    console.log(this.cvAssessment);
    if (this.cvAssessment !== null) return;
    
    var cvAssess: ICandidateAssessment;
    var cvItems: ICandidateAssessmentItem[]=[];
    if (!this.requireInternalReview) {    //create assessment header without assessment question items
      cvAssess = new CandidateAssessment(this.orderItemSelectedId, this.cvBrief!.id, this.user!.loggedInEmployeeId,  this.user!.displayName, 
          new Date(), this.orderItemSelected!.requireInternalReview, this.checklist!.id, cvItems );
    } else { 
      var items: IOrderItemAssessmentQ[];
      this.service.getOrderItemAssessmentQs(this.orderItemSelectedId).subscribe(response => {
        
          items = response;
          if (items.length === 0) {
            this.toastr.warning('The Order Item selected requires internal assessment of candidates, for which ' + 
              'assessment Questions for the Order Item must be designed.  The Selected order item has not been designed');
            this.orderItemChangedEventSubject.next(undefined);        
            return null;
          }
          
          items.forEach(i => {
            var cItem = new CandidateAssessmentItem(i.questionNo,i.isMandatory, i.question, i.maxMarks);
            cvItems.push(cItem);
          })
          this.cvAssessment = new CandidateAssessment(this.orderItemSelectedId, this.cvBrief!.id, this.user!.loggedInEmployeeId, 
              this.user!.displayName,  new Date(), this.orderItemSelected!.requireInternalReview, this.checklist!.id, cvItems );
            this.orderItemChangedEventSubject.next(this.cvAssessment);   
          
          return;
      }, error => {
          this.toastr.error('failed to create Assessment item', error);
          this.orderItemChangedEventSubject.next(this.cvAssessment);
          return null;
      })
    }
  }

  updateAssessment(assess: ICandidateAssessment) {
    
   //check data
    
    return this.service.updateCVAssessment(assess).subscribe(response => {
        if (response) {
          this.toastr.success('updated the Candidate Assessment');
          //this.cvAssessment=null;
          //this.orderItemChangedEventSubject.next(null);
        } else {
          this.toastr.warning('failed to update the candidate assessment');
        }
      }, error => {
        this.toastr.error('failed to update the candidate assessment', error);
      })
   
    
  }

  shortlistForForwarding() {
    if(!this.checklist) {
      this.toastr.warning('no checklisting done on the candidate');
      return;
    }
    if (this.cvAssessment !==null) {
      this.toastr.warning('this candidate is already assessed.');
      return;
    }

    //create new CV Assessment
      return this.service.insertCVAssessmentHeader(this.requireInternalReview, 
          this.cvBrief!.id, this.orderItemSelectedId, new Date()).subscribe(response => {
        
        if ((response.errorString==='' || response.errorString ===undefined || response.errorString === null) ) {
          
          this.cvAssessment = response.candidateAssessment;
          
          //update existingAssessmentsDto to update new assessment on the screen
          if (this.cvAssessment !==null) {
            //this.patchForm(this.cvAssessment);    //this is done in child form
              var dto = new CandidateAssessedDto(); // = new AssessmentOfACandidateDto();
              dto.checklistedByName = "to fix";
              dto.checklistedOn = new Date();
              dto!.customerName = "to fix";
              dto!.categoryName = "to fix";
              dto!.orderItemRef = "to fix";
              dto!.assessedOn = this.cvAssessment.assessedOn;
              dto!.assessedByName = this.user!.displayName;

              this.existingAssessmentsDto.push(dto);
              this.toastr.success('shortlisted for forwarding to client');
          }
        } else if (response.errorString !== '') {
          this.toastr.error('failed to shortlist the candidate, ', response.errorString);
          console.log(response.errorString);
        }
      }, error => {
        this.toastr.error('error in creating the shortlisting', error);
        this.validationErrors = error;
      })
    
  }

  openShortlistCandidateHelpModal() {
    
    if(this.helpfile===null) {
      this.toastr.info("No help file available for the topic 'Candidate Assessment'");    
      return;
    }

    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        help: this.helpfile,
      }
    }
    this.bsModalRef = this.bsModalService.show(HelpModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(() => {
    })
  }
  
  private getHelp(topic: string): IHelp|null {
    this.confirmService.getHelp(topic).subscribe(response => {
      this.helpfile = response;
      return;
    }, error => {
      console.log('failed to retrieve roles array', error);
    })
    return null;
  }

  //checklist modal
  openChecklistModal() {
      if (this.orderItemSelected === null || this.orderItemSelected === undefined) {
        this.toastr.warning('Order Item not selected');
        return;
      } else if (this.cvBrief!.id === 0) {
        this.toastr.warning("Candidate Id not provided");
        return;
      }
      
      //this.checklist = this.getChecklistHRDto(this.cvBrief.id, this.orderItemSelectedId);
      this.checklistitems = this.checklist!.checklistHRItems;

      if (this.checklist === undefined || this.checklist === null) {
        this.toastr.warning("failed to get checklist values");
        return;
      }

      const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
          chklst: this.checklist //this.getChecklistHRDto(this.cvBrief.id, this.orderItemSelectedId),
        }
      }
    
      this.bsModalRef = this.bsModalService.show(ChecklistModalComponent, config);
      this.bsModalRef.content.updateChecklist.subscribe((values: any) => {
        this.checklist = values;
        console.log('checklist', this.checklist, 'cvAssessment:', this.cvAssessment);
        if (this.cvAssessment) this.cvAssessment.hrChecklistId=this.checklist!.id;
        
        var checklistErrors = this.CheckChecklist(this.checklist!);
        this.checklist!.checklistedOk=checklistErrors===null || checklistErrors.length===0;
          
          this.checklistService.updateChecklist(this.checklist!).subscribe(() => {
              this.toastr.success('updated Checklist');
          }, error => {
            this.toastr.error('failed to update the checklist', error);
          });
        } 
      )
    }

    CheckChecklist(model: IChecklistHRDto) {
      var errors: string[]=[];
      if(model.charges > 0 && model.chargesAgreed != model.charges && !model.exceptionApproved ) errors.push('Charges not agreed');
      model.checklistHRItems.forEach(p => {
        if(p.mandatoryTrue && !p.accepts) errors.push(p.parameter + ' is mandatory, and not resolved');
      })

      return errors;
    }

    EmitOrderItemChanged() {
      //this.orderItemChangedEventSubject.next(this.orderItemSelected);
    }

    DisplayHelp() {
      this.bDisplayHelp = !this.bDisplayHelp;
      this.displayText = this.bDisplayHelp ? 'Hide help text' : 'display Help Text';
    }

}
