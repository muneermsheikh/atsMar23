import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';
import { IProspectiveRegisterToAddDto } from 'src/app/shared/dtos/hr/prospectiveRegisterToAddDto';
import { IProspectiveUpdateDto, ProspectiveUpdateDto } from 'src/app/shared/dtos/hr/prospectiveUpdateDto';
import { IContactResult } from 'src/app/shared/models/admin/contactResult';
import { IUser } from 'src/app/shared/models/admin/user';
import { IProspectiveCandidate } from 'src/app/shared/models/hr/prospectiveCandidate';
import { prospectiveCandidateParams } from 'src/app/shared/params/hr/prospectiveCandidateParams';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { ProspectiveService } from '../prospective.service';

@Component({
  selector: 'app-prospective-list',
  templateUrl: './prospective-list.component.html',
  styleUrls: ['./prospective-list.component.css']
})
export class ProspectiveListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  recordChanged:boolean=false;

  user?: IUser;

  form: FormGroup = new FormGroup({});
  
  prospectives: IProspectiveCandidate[]=[];
  //employees: IEmployeeIdAndKnownAs[];
  results: IContactResult[]=[];

  prospectParams = new prospectiveCandidateParams();
  statusParams: string = 'pending';

  routeStatus:string='';
  totalCount: number=0;
  returnUrl: string='';
  bNavigationExtras: boolean=false;

  constructor(private fb: FormBuilder, 
      private service: ProspectiveService,
      private activatedRoute: ActivatedRoute,
      private toastr: ToastrService,
      private accountService: AccountService,
      private confirmService: ConfirmService,
      private router: Router
    ) {
      let nav: Navigation | null = this.router.getCurrentNavigation();
      if (nav?.extras && nav.extras?.state?.returnUrl) {
          this.bNavigationExtras=true;
          this.returnUrl=nav.extras.state.returnUrl as string;
      }

      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
      
      //var catRef = this.routeStatus=activatedRoute.snapshot.paramMap.get('categoryRef');
      var catRef = activatedRoute.snapshot.paramMap.get('categoryRef');

      //var dt = this.routeStatus=activatedRoute.snapshot.paramMap.get('dated');
      var dt = activatedRoute.snapshot.paramMap.get('dated');

      //var st = this.routeStatus=activatedRoute.snapshot.paramMap.get('status');
      var st = activatedRoute.snapshot.paramMap.get('status');

      this.statusParams= st==='' ? 'all' : this.statusParams;
      if(dt !==null) this.prospectParams.dateAdded=dt;
      if(catRef !== null) this.prospectParams.categoryRef=catRef;
      if(st !== null) this.prospectParams.status=st;
      
      this.service.setParams(this.prospectParams);
    }

    ngOnInit(): void {

      this.activatedRoute.data.subscribe(data => { 
        this.results = data.results;
        //this.totalCount = data.summary.count;
      
      })
      
      //the candiadteParams are already setup before calling this page
      this.getProspectives(false);
      this.createAndPatchForm();

    }


    createAndPatchForm() {
      this.form = this.fb.group({
          prospectiveCandidates: this.fb.array([]),
        } 
      );

      //console.log('prospectlist coponent.ts, prospectives', this.prospectives);

      if(this.prospectives !== null && this.prospectives !== undefined) {
        this.form.setControl('prospectiveCandidates', this.setExistingCandidates(this.prospectives));
      }
    }

    
    setExistingCandidates(items: IProspectiveCandidate[]): FormArray {
      const formArray = new FormArray([]);
      items.forEach(p => {
        formArray.push(this.fb.group({
          id: p.id,  
          checked: p.checked, 
          categoryRef: p.categoryRef,
          source: p.source, 
          resumeId: p.resumeId, nationality: p.nationality, address: p.address,
          city: p.city, date: p.date, candidateName: p.candidateName, age:p.age, 
          phoneNo: p.phoneNo, alternatePhoneNo: p.alternatePhoneNo, email: p.email, 
          alternateEmail: p.alternateEmail, currentLocation: p.currentLocation,
          workExperience: p.workExperience, status: p.status, newStatus: p.newStatus,
          statusDate: p.statusDate, statusByUserId:p.statusByUserId, userName: p.userName,
          closed: p.closed, remarks: p.remarks
        }))
      });
      return formArray;
  }

    get prospectiveCandidates() : FormArray {
      return this.form.get("prospectiveCandidates") as FormArray
    }

    removeItem(i:number) {
      this.prospectiveCandidates.removeAt(i);
      this.prospectiveCandidates.markAsDirty();
      this.prospectiveCandidates.markAsTouched();
    }

    getProspectives(useCache=false) {

      return this.service.getProspectiveCandidates(useCache).subscribe((response: any) =>{
        this.prospectives = response.data;
        this.totalCount=response.count;

        this.form.setControl('prospectiveCandidates', this.setExistingCandidates(this.prospectives));

      }, (error: any) => {
        console.log('error', error);
      })
    }

    
  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getProspectives();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.prospectParams = new prospectiveCandidateParams();
    this.service.setParams(this.prospectParams);
    this.getProspectives();
  }
  
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      if(this.form.dirty) {
        if(!this.confirmService.confirm('Changes not saved', 
          'You have made changes to this form; moving to another page without saving the changes will lose the edits', 
          'Ok, abort changes', 
          'No, cancel moving to other page'))
          this.toastr.info('moving to other page aborted');
      }

      params.pageNumber = event;
      this.service.setParams(params);
      this.getProspectives(true);

      this.recordChanged=false;
      
    }
  }

  onStatusChanged(val: any) {
    console.log('status value',val);
  }

  statusSelected(st: string) {
    var serviceProsParams = this.service.getParams();
    if(serviceProsParams.status===st) return;

    serviceProsParams.status = st;

    this.service.setParams(serviceProsParams);

    this.getProspectives();

  }

  statusPendingClicked() {
    this.prospectParams.status="Pending";
    this.service.setParams(this.prospectParams);
    this.statusParamsRefresh();
  }

  statusConcludedClicked() {
    this.prospectParams.status="Concluded";
    this.service.setParams(this.prospectParams);
    this.statusParamsRefresh();
  }

  statusAllClicked() {
    this.prospectParams.status="";
    this.service.setParams(this.prospectParams);
    this.statusParamsRefresh();
  }

  statusParamsRefresh(){
    var serviceProsParams = this.service.getParams();
    serviceProsParams.status = this.prospectParams.status;
    this.service.setParams(serviceProsParams);

    this.getProspectives();
  }
  
  Update() {
    var dtos: IProspectiveUpdateDto[]=[];
    this.prospectiveCandidates.value.filter((x: any) => x.closed).forEach((element: any) => {element.newStatus='Concluded'});

    this.prospectiveCandidates.value.filter((x: any) => x.newStatus !== '').forEach((element: any) => {
      var dto = new ProspectiveUpdateDto();
      dto.newStatus=element.newStatus;
      dto.remarks = element.remarks;
      dto.prospectiveId=element.id;
      dtos.push(dto);  
    });

    if(dtos.length ==0) {
      this.toastr.warning('no record changed');
      return;
    }

    this.service.updateProspectives(dtos).subscribe(() => {
      this.toastr.success('prospective candidate updates made');
        
    }, (error: any) => {
      this.toastr.error('failed to update the prospectives', error);
      console.log('error in Update', error);
    })
  }
  
  showUserTask(resumeid: string, cat: string) {
    //console.log('showuertask resumeid', resumeid);
    //this.router.navigateByUrl("/userTask/viewbyresumeid/" + resumeid);

    var dt = new Date().toString();
    var st = 'all';

    let route = '/userTask/viewbyresumeid/' + resumeid;
    this.router.navigate([route], { state: { returnUrl: '/prospectives/prospectivelist/' + cat + '/' + dt + '/' + st } });

  }
  
  transferProfileToCV(cv: IProspectiveCandidate) {
    if (cv.source === undefined || cv.candidateName===undefined || cv.email === undefined 
       || cv.currentLocation === undefined || cv.phoneNo === undefined || cv.categoryRef === undefined ) {
         this.toastr.info('required values missing');
         return;
       }
 
     var dto: IProspectiveRegisterToAddDto = {} as IProspectiveRegisterToAddDto;
     dto.candidateName=cv.candidateName;
     dto.age=cv.age;
     dto.prospectiveId=cv.id;
     dto.gender='Male';
     dto.knownAs=cv.candidateName.substring(0,15);
     dto.userName=this.user?.displayName!;
     dto.password="pr0$pectivE";
     dto.email=cv.email;
     dto.categoryRef = cv.categoryRef;
     dto.source=cv.source;
     dto.currentLocation=cv.currentLocation;
     dto.phoneNo=cv.phoneNo;
     dto.alternatePhoneNo=cv.alternatePhoneNo;
 
     return this.service.createCandidateFromprospective(dto).subscribe(() => {
       this.toastr.success('converted to candidate object');
       //remove the record from prospective;
       
       var profRecordIndex = this.prospectives.findIndex(x => x.id === cv.id);
       this.prospectives.splice(profRecordIndex,1);
       --this.totalCount;
 
     }, (error: any) => {
       console.log('failed to convert the object to candidate', error);
     })
   }

   Refresh() {
    this.getProspectives(false);
   }

   returnToCalling() {
    this.router.navigateByUrl(this.returnUrl || '' );
  }
   
  closeChanged(e:any, index: number, id: number) {
    var changedVal = e.target.checked;
    this.prospectiveCandidates.at(index).get('closed')!.setValue(changedVal);
    if(changedVal) {
      if(changedVal && this.prospectiveCandidates.at(index).get('status')!.value !=='Concluded') 
      this.prospectiveCandidates.at(index).get('status')!.setValue('Concluded');
    } else {
      var oldVal = this.prospectives.map(x => x.status);

      console.log('oldval', oldVal);
      //this.prospectiveCandidates.at(index).get('status').setValue(oldVal);
    }
    
  }
}
