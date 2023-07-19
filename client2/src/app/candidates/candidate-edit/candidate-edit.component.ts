import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ICandidate } from 'src/app/shared/models/hr/candidate';
import { IProfession} from 'src/app/shared/models/masters/profession';
import { CandidateService } from '../candidate.service';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { AccountService } from 'src/app/account/account.service';
import { ToastrService } from 'ngx-toastr';
import { map, switchMap } from 'rxjs/operators';
import { of, timer } from 'rxjs';
import { IUserQualification } from 'src/app/shared/params/admin/userQualification';
import { IUserProfession } from 'src/app/shared/params/admin/userProfession';
import { IUserExp } from 'src/app/shared/models/hr/userExp';
import { IUserAttachment } from 'src/app/shared/models/hr/userAttachment';
import { IUserPhone } from 'src/app/shared/params/admin/userPhone';
import { IQualification } from 'src/app/shared/models/hr/qualification';
import { ICustomerNameAndCity } from 'src/app/shared/models/admin/customernameandcity';
import { MastersService } from 'src/app/masters/masters.service';
import { IApiReturnDto } from 'src/app/shared/dtos/admin/apiReturnDto';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-candidate-edit',
  templateUrl: './candidate-edit.component.html',
  styleUrls: ['./candidate-edit.component.css']
})
export class CandidateEditComponent implements OnInit {

  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent | undefined;
  activeTab: TabDirective | undefined;

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;
  memberPhotoUrl: string='';

  candidate: ICandidate|undefined;
  professions: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  qualifications: IQualification[]=[] ;
  
  errors: string[]=[];

  //id: string='';
  isAddMode: boolean=false;
  loading = false;
  submitted = false;
  bsValueDate = new Date();

  fileToUpload: File | null = null;
  filesToUpload: File[] = [];

  selectedProfession='';
  events: Event[] = [];

  //file upload
  //source:
  @Output() public onUploadFinished = new EventEmitter();
  //isCreate: boolean;
  progress = { loaded : 0 , total : 0 };

  //public progress: number=0;
  public message: string='';  
  isMultipleUploaded = false;
  isSingleUploaded = false;
  urlAfterUpload = '';
  percentUploaded = 0;
  attachmentType: string = '';
  userFiles: File[] = [];
  imageFile: File | undefined;
  response: { dbPath: ''; } | undefined;
  attachmentid: number=0;
  
  routeId='';
  user?: IUser;
  returnUrl = '/candidates';

  attachmentTypes = [
    {'typeName':'CV'}, {'typeName':'Educational Certificate'},{'typeName':'Experience Certificate'}, {'typeName':'Passport Copy'}, {'typeName':'Photograph'}  
  ]
  
  lastTimeCalled: number= Date.now();
  //end of file upload

  constructor(
    private accountService: AccountService, 
    private toastrService: ToastrService, 
    private fb: FormBuilder, 
    private activatedRoute: ActivatedRoute,
    private mastersService: MastersService,
    private candidateService: CandidateService,
    private router: Router
    ) {
      
          this.routeId = this.activatedRoute.snapshot.params['id'];
          //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

          this.routeId = this.activatedRoute.snapshot.params['id'];
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
    }

  ngOnInit(): void {

    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);

    this.activatedRoute.data.subscribe( data => {
        this.professions = data.categories,
        this.qualifications = data.qualifications,
        this.agents = data.agents,
        this.candidate = data.candidate;

        this.isAddMode = this.candidate?.id===0;

        if(this.candidate !== undefined) {
          this.editCandidate(this.candidate);
        }
    });

    //this.getQualifications();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
        id: 0,
        userType: ['candidate', Validators.required],
        applicationNo:0,        
        gender: ['male', Validators.required],
        nationality: ['Indian', Validators.required],
        knownAs: ['', Validators.required],
        username: '',
        firstName: ['', Validators.required],
        secondName: '',
        familyName: '',
        dOB: '',
        placeOfBirth: '',
        aadharNo: '',
        photoUrl: '',
        ppNo: '',
        ecnr: [false],
        referredBy: 0,
        referredByName: '',
        
        password: ['']  , /*[Validators.required, 
          Validators.minLength(4), Validators.maxLength(8)]], */
        confirmPassword: '',  // [Validators.required, this.matchValues('password')]],
        
      //}),
      //userAddressForm: this.fb.group({
        address: '',
        address2: '',
        city: ['', Validators.required],
        pin: '',
        district:'',
        country: ['India'],
        email: ['', Validators.email],
      //})
      
      userPhones: this.fb.array([]),
      userQualifications: this.fb.array([]), 
      userProfessions: this.fb.array([]),
      //userPassports: this.fb.array([]),
      //entityAddresses: this.fb.array([]),
      userExperiences: this.fb.array([]),
      userAttachments: this.fb.array([])

    });

    if(this.registerForm.controls['password']) {
      this.registerForm.controls['password'].valueChanges.subscribe({
        next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })

    }
  }

  //edit form

  editCandidate(cv: ICandidate) {
    //const dob = this.getDateOnly(this.registerForm.controls['dOB'].value!);
    //const values = {...this.registerForm.value!, dateOfBirth: dob!};
    console.log('patching up values', cv);

    this.registerForm.patchValue( {
      id: cv.id, userType: cv.userType , applicationNo: cv.applicationNo, gender: cv.gender, firstName: cv.firstName,
      secondName: cv.secondName, familyName: cv.familyName, knownAs: cv.knownAs, referredBy: cv.referredBy, dOB: cv.dOB,
      placeOfBirth: cv.placeOfBirth, aadharNo: cv.aadharNo, ppNo: cv.ppNo, ecnr: cv.ecnr, city: cv.city, pin: cv.pin,
      nationality: cv.nationality, email: cv.email, companyId: cv.companyId, introduction: cv.introduction, 
      interests: cv.interests, notificationDesired: cv.notificationDesired
    });
      if(cv.photoUrl !== null) this.memberPhotoUrl = 'https://localhost:5001/api/assets/images/' + cv.photoUrl;
      
     if (cv.userPhones !== null) this.registerForm.setControl('userPhones', this.setExistingPhones(cv.userPhones));
      if (cv.userQualifications !== null) this.registerForm.setControl('userQualifications', this.setExistingQ(cv.userQualifications));
      if (cv.userProfessions !== null) this.registerForm.setControl('userProfessions', this.setExistingProfs(cv.userProfessions));
      if (cv.userExperiences !== null) this.registerForm.setControl('userExperiences', this.setExistingExps(cv.userExperiences));
      if (cv.userAttachments !== null) this.registerForm.setControl('userAttachments', this.setExistingAttachments(cv.userAttachments));
    
  }
  setExistingPhones(userphones: IUserPhone[]): FormArray {
    const formArray = new FormArray([]);
    userphones.forEach(ph => {
      formArray.push(this.fb.group({
        id: ph.id,
        candidateId: ph.candidateId,
        mobileNo: ph.mobileNo,
        isMain: ph.isMain,
        remarks: ph.remarks,
        isValid: ph.isValid
      }))
    });
    return formArray;
}

  setExistingQ(userQ: IUserQualification[]): FormArray {
      const formArray = new FormArray([]);
      userQ.forEach(q => {
        formArray.push(this.fb.group({
          id: q.id,
          candidateId: q.candidateId,
          qualificationId: q.qualificationId,
          qualification: q.qualification,
          isMain: q.isMain
        }))
      });
      return formArray;
  }

  setExistingProfs(userProfs: IUserProfession[]): FormArray {
    const formArray = new FormArray([]);
    userProfs.forEach(p => {
      console.log('setExistingProfs', p);
      formArray.push(this.fb.group({
        id: p.id,
        candidateId: p.candidateId,
        categoryId: p.categoryId,
        profession: p.profession,
        isMain: p.isMain
      }))
    });
    return formArray;
  }  

  setExistingExps(userExps: IUserExp[]): FormArray {
    const formArray = new FormArray([]);
    userExps.forEach(p => {
      formArray.push(this.fb.group({
        id: p.id,
        candidateId: p.candidateId,
        employer: p.employer,
        position: p.position,
        positionId: p.positionId,
        salaryCurrency: p.salaryCurrency,
        monthlySalaryDrawn: p.monthlySalaryDrawn,
        workedFrom: p.workedFrom,
        workedUpto: p.workedUpto
      }))
    });
    return formArray;
  }  

  setExistingAttachments(userAttachs: IUserAttachment[]): FormArray {
    const formArray = new FormArray([]);
    userAttachs.forEach(p => {
      formArray.push(this.fb.group({
        id: p.id,
        candidateId: p.candidateId,
        appUserId: p.appUserId,
        fileName: p.fileName,
        attachmentSizeInBytes: p.attachmentSizeInBytes,
        url: p.url,
        attachmentType: p.attachmentType
    }))
    });
    return formArray;
  } 
  //get data from api
  getCVById(id: number) {
      return this.candidateService.getCandidate(id).subscribe({
        next: response => {
          this.candidate = response;
          //this.candidate  = {...this.candidate, userType: 'candidate'};
          this.editCandidate(this.candidate);
        },
        error: error => this.toastrService.error('failed to get candidate to edit:', error)
      })
  }

   getQualifications(){
    return this.mastersService.getQualificationList().subscribe({
      next: (response: any) => this.qualifications=response,
      error: (error: any) => this.toastrService.error('error in qualificatin', error)
    })
  }
  
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
    this.router.navigateByUrl(this.returnUrl);
  }


  selectTab(tabId: number) {
    this.memberTabs!.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return control => {
      //return timer(500).pipe(
      return timer(10).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkEmailExists(control.value).pipe(
            map(res => {
              return res ? {emailExists: true} : null;
            })
          );
        })
      )
    }
  }

  validatePPNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(10).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkPPExists(control.value).pipe(
            map(res => {
              if (res !== null) this.toastrService.warning('that passport number is taken by ' + res);
              return res ? {ppExists: true} : null;
            })
          );
        })
      )
    }
  }

  handleFileInput(files: any) {
    //var f: files.target.files;
    //this.fileToUpload = files.item(0);
    this.fileToUpload = files[0];
    this.filesToUpload.push(files[0]);
  }

  getAgentValue(agt: ICustomerNameAndCity)
  {
    console.log('agent selected', agt);
  }

  getProfessionValue(prof: IProfession) {
    console.log('ngx dropdown selected', prof);
  } 
  
  //UserPhones
  get userPhones() : FormArray {
    return this.registerForm.get("userPhones") as FormArray
  }
  newUserPhone(): FormGroup {
    return this.fb.group({
      mobileNo: ['', Validators.required],
      isMain: false,
      remarks: ''
    })
  }
  addPhone() {
    this.userPhones.push(this.newUserPhone());
  }
  removeUserPhone(i:number) {
    this.userPhones.removeAt(i);
  }

  //userQualifications
  get userQualifications() : FormArray {
    return this.registerForm.get("userQualifications") as FormArray
  }
  newQualification(): FormGroup {
    return this.fb.group({
      qualificationId: 0,
      qualificationName: '',
      isMain: [false]
    })
  }
  addQualification() {
    this.userQualifications.push(this.newQualification());
  }
  removeUserQualification(i:number) {
    this.userQualifications.removeAt(i);
  }

  //userProfessions
  get userProfessions() : FormArray {
        return this.registerForm.get("userProfessions") as FormArray
  }

  newUserProfession(): FormGroup {
    return this.fb.group({
      categoryId: [0, Validators.required],
      isMain: [false, Validators.required]
    })
  }

  addUserProfession() {
    this.userProfessions.push(this.newUserProfession());
  }

  removeUserProfession(i:number) {
    this.userProfessions.removeAt(i);
  }

  //userExp
  get userExperiences() : FormArray {
    return this.registerForm.get("userExperiences") as FormArray
  }
  
  newUserExp(): FormGroup {
    return this.fb.group({
      srNo: [0, Validators.required],
      position: ['', Validators.required],
      company: ['', Validators.required],
      workedFrom: '',
      workedUpto: ''
    })
  }

  addUserExp() {
    this.userExperiences.push(this.newUserExp());
  }

  removeUserExp(i:number) {
    this.userExperiences.removeAt(i);
  }
    
  //userAttachments
  get userAttachments() : FormArray {
    return this.registerForm.get("userAttachments") as FormArray
  }
  newUserAttachment(): FormGroup {
    return this.fb.group({
      candidateId: this.candidate===undefined ? 0 : this.candidate.id,
      attachmentType: ['', Validators.required],
      fileName: ['', Validators.required],
      attachmentSizeInBytes: 0,
      url: ''
    })
  }

  addUserAttachment() {
    this.userAttachments.push(this.newUserAttachment());
  }
  removeUserAttachment(i:number) {
    this.userAttachments.removeAt(i);
  }

  getValues( $event: any) {
    console.log('ngx dropdown selected', this.selectedProfession);
  }

  onFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    this.userFiles.push(f);
  }

  savewithattachments = () => {
    var microsecondsDiff: number= 28000;
    var nowDate: number =Date.now();
    
    if(nowDate < this.lastTimeCalled+ microsecondsDiff) return;
    
    this.lastTimeCalled=Date.now();
    const formData = new FormData();
    const formValue = this.registerForm.value;

    
    if(this.userFiles.length > 0) {
      this.userFiles.forEach(f => {
        formData.append('file', f, f.name);
      })
    }

    formData.append('data', JSON.stringify(this.registerForm.value));

    if(this.candidate=== undefined || this.candidate?.id === 0) {   //insert new cv
      this.toastrService.info('inserting ...');
        this.candidateService.registerWithFiles(formData).subscribe({
          next: (response: IApiReturnDto) => {
            
            if(response.errorMessage!==null) {
              this.toastrService.error('failed to save the candidate data', response.errorMessage);
            } else {
              this.toastrService.success('candidate saved, with Application No. ' + response.returnInt.toString(), 'Profile successfully inserted');
            }},
          error: error => this.toastrService.error('failed to save the candidate', error)
    })} else {
        this.toastrService.info('udating ...');
        this.candidateService.UpdateCandidateWithFiles(formData).subscribe({
          next: (response: string) => {
            if(response !== '') {
              this.toastrService.error('failed to update the candidate', response);
            } else {
              this.toastrService.success('updated the candidate successfully');
              this.router.navigateByUrl(this.returnUrl);
            }},
            error: error => {
              this.toastrService.error('failed to update the candidate', error)
              console.log(error);
            }
          })
    }
}


  uploadFinished = (event: any) => { 
    this.response = event; 
  }


}
