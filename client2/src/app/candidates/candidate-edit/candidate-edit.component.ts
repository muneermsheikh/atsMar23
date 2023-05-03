import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormArray, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { IUser } from 'src/app/shared/models/admin/user';
import { Candidate, ICandidate } from 'src/app/shared/models/hr/candidate';
import { IProfession, IQualification } from 'src/app/shared/models/masters/profession';
import { CandidateService } from '../candidate.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute } from '@angular/router';
import { AccountService } from 'src/app/account/account.service';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, map, switchMap, take } from 'rxjs/operators';
import { of, timer } from 'rxjs';
import { FileService } from 'src/app/shared/services/file.service';
import { HttpErrorResponse, HttpEventType, HttpResponse } from '@angular/common/http';
import { IUserQualification } from 'src/app/shared/params/admin/userQualification';
import { IUserProfession } from 'src/app/shared/params/admin/userProfession';
import { IUserExp } from 'src/app/shared/models/hr/userExp';
import { IUserAttachment } from 'src/app/shared/models/hr/userAttachment';
import { IUserPhone } from 'src/app/shared/params/admin/userPhone';

@Component({
  selector: 'app-candidate-edit',
  templateUrl: './candidate-edit.component.html',
  styleUrls: ['./candidate-edit.component.css']
})
export class CandidateEditComponent implements OnInit {

  response: { dbPath: ''; } | undefined;

  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent | undefined;
  activeTab: TabDirective | undefined;
  @ViewChild('editForm') editForm: NgForm | undefined;
  
  routeId: string;
  member: ICandidate | undefined;
  user: IUser | undefined;
  
  form: FormGroup =new FormGroup({});

  selectedCategoryIds: number[]=[];
  categories: IProfession[]=[];
  qualifications: IQualification[]=[];
  
  //events: Event[] = [];

  isAddMode: boolean=false;
  loading = false;
  submitted = false;

  errors: string[]=[];

  bsValue = new Date();
  bsRangeValue: Date[];
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  //source:
  @Output() public onUploadFinished = new EventEmitter();
  //isCreate: boolean;
  public progress: number=0;
  public message: string='';  
  isMultipleUploaded = false;
  isSingleUploaded = false;
  urlAfterUpload = '';
  percentUploaded = [0];
  attachmentType: string = '';

  memberPhotoUrl: string='';
  
  userFiles: File[] = [];
  imageFile: File | undefined;

  attachmentid: number=0;
  attachmentTypes = [
    {'typeName':'CV'}, {'typeName':'Educational Certificate'},{'typeName':'Experience Certificate'}, {'typeName':'Passport Copy'}, {'typeName':'Photograph'}  
  ]
  //end of 
  
  lastTimeCalled: number= Date.now();

  constructor(private service: CandidateService, 
      private bcService: BreadcrumbService, 
      private activatedRoute: ActivatedRoute, 
      private fileService: FileService,
      private accountService: AccountService, 
      private toastr: ToastrService, 
      private fb: FormBuilder) {
    this.bcService.set('@candidateDetail',' ');
    this.routeId = this.activatedRoute.snapshot.params['id'];
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 20);  //10 years later
    this.minDate.setFullYear(this.minDate.getFullYear() + 20);
    this.bsRangeValue = [this.bsValue, this.maxDate];
   }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.categories = data.categories;
      this.qualifications = data.qualifications;
      this.member = data.candidate;
      //this.isAddMode = this.member!==null;
      this.isAddMode = this.member ===null || this.member===undefined;
      if(this.isAddMode) this.member=new Candidate();
    })

      this.createForm();
      if (!this.isAddMode) this.editCandidate(this.member!);
  }
  /*
  required; gender, firstname, knownas, email, city, password, confirmpassword, notificationdesired, userPhones.MobileNo, userprofessions.cateoryId
  userExp: workedfrom, emloyer, position
  */  
  createForm() {
      this.form = this.fb.group({
        id: 0,
        userType: 'candidate',
        applicationNo: [0],
        gender: ['M', [Validators.required, Validators.maxLength(1)]],
        firstName: ['', [Validators.required, Validators.maxLength(25)]],
        secondName: '',
        familyName: '',
        knownAs: ['', Validators.required],
        referredBy: 0,
        referredByName: '',
        dOB: null,
        placeOfBirth: null,
        aadharNo:'',
        passportNo: '',
        ecnr: false,
        nationality: ['Indian'],
        email: [null, 
          [Validators.required, Validators
          .pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')],
          [this.validateEmailNotTaken()]
        ],
        address: '',
        city: ['', Validators.required],
        pin: ['', [Validators.maxLength(6), Validators.minLength(6)]],
        password: ['', this.isAddMode ? Validators.required : Validators.nullValidator],
        //confirmPassword: ['', this.isAddMode ? Validators.required : Validators.nullValidator],
        confirmPassword: ['', [this.isAddMode ? Validators.required : Validators.nullValidator]], //this.matchValues('password')]],
        companyId: [null],
        introduction: [''],
        interests: [''],
        notificationDesired: [false, Validators.required],
        userPhones: this.fb.array([]),
        userQualifications: this.fb.array([]), 
        userProfessions: this.fb.array([]),
        userExperiences: this.fb.array([]),
        userAttachments: this.fb.array([]),
      }  //{validator: MustMatch('password', 'confirmPassword')}
      );
    }

    matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
        return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
    }

    private getDateOnly(dob: string | undefined) {
      if (!dob) return;
      let theDob = new Date(dob);
      return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset()))
        .toISOString().slice(0,10);
    }

    editCandidate(cv: ICandidate) {
      
      const dob = this.getDateOnly(this.form.controls['dOB'].value);
      const values = {...this.form.value, dateOfBirth: dob};

      this.form.patchValue( {
        id: cv.id, userType: cv.userType , applicationNo: cv.applicationNo, gender: cv.gender, firstName: cv.firstName,
        secondName: cv.secondName, familyName: cv.familyName, knownAs: cv.knownAs, referredBy: cv.referredBy, dOB: cv.dOB,
        placeOfBirth: cv.placeOfBirth, aadharNo: cv.aadharNo, ppNo: cv.ppNo, ecnr: cv.ecnr, city: cv.city, pin: cv.pin,
        nationality: cv.nationality, email: cv.email, companyId: cv.companyId, introduction: cv.introduction, 
        interests: cv.interests, notificationDesired: cv.notificationDesired
      });
      if(cv.photoUrl !== null) this.memberPhotoUrl = 'https://localhost:5001/api/assets/images/' + cv.photoUrl;
      
      if (cv.userPhones !== null) this.form.setControl('userPhones', this.setExistingPhones(cv.userPhones));
      if (cv.userQualifications !== null) this.form.setControl('userQualifications', this.setExistingQ(cv.userQualifications));
      if (cv.userProfessions !== null) this.form.setControl('userProfessions', this.setExistingProfs(cv.userProfessions));
      if (cv.userExperiences !== null) this.form.setControl('userExperiences', this.setExistingExps(cv.userExperiences));
      if (cv.userAttachments !== null) this.form.setControl('userAttachments', this.setExistingAttachments(cv.userAttachments));
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
    
    
//userPhones
      get userPhones() : FormArray {
        return this.form.get("userPhones") as FormArray
      }

      newPhone(): FormGroup {
        return this.fb.group({
          mobileNo: ['', Validators.required],
          isMain: false,
          isValid: true,
          remarks: ''
        })
      }
      addPhone() {
        this.userPhones.push(this.newPhone());
      }
      removeUserPhone(i:number) {
        this.userPhones.removeAt(i);
        this.userPhones.markAsDirty();
        this.userPhones.markAsTouched();
      }

  //userQualifications
      get userQualifications() : FormArray {
        return this.form.get("userQualifications") as FormArray
      }

      newUserQualification(): FormGroup {
        return this.fb.group({
          candidateId: 0,
          qualificationId: 0,
          qualification: '',
          isMain: false
        })
      }
      addQualification() {
        this.userQualifications.push(this.newUserQualification());
      }

      removeUserQualification(i:number) {
        this.userQualifications.removeAt(i);
        this.userQualifications.markAsDirty();
        this.userQualifications.markAsTouched();
      }

// userProfessions
      get userProfessions() : FormArray {
        return this.form.get("userProfessions") as FormArray
      }

      newUserProfession(): FormGroup {
        return this.fb.group({
          id: 0,
          candidateId: 0,
          profession: '',
          categoryId: [0, Validators.required],
          industryId: 0,
          isMain: false
        })
      }

      addUserProfession() {
        this.userProfessions.push(this.newUserProfession());
      }  
      removeUserProfession(i:number) {
        this.userProfessions.removeAt(i);
        this.userProfessions.markAsDirty();
        this.userProfessions.markAsTouched();
      }
    
  //user exp
      get userExperiences() : FormArray {
        return this.form.get("userExperiences") as FormArray
      }
      newUserExperience(): FormGroup {
        return this.fb.group({
          candidateId: 0,
          srNo: 0,
          employer: ['', Validators.required],
          positionId: 0,
          position: ['', Validators.required],
          salaryCurrency: '',
          monthlySalaryDrawn: 0,
          workedFrom: [null, Validators.required],
          workedUpto: [null]
        })
      }
      addUserExperience() {
        this.userExperiences.push(this.newUserExperience());
      }  

      removeUserExperience(i:number) {
        this.userExperiences.removeAt(i);
        this.userExperiences.markAsDirty();
        this.userExperiences.markAsTouched();
      }

      //userAttachments
      get userAttachments() : FormArray {
        return this.form.get("userAttachments") as FormArray
      }

      newUserAttachment(): FormGroup {
        return this.fb.group({
          id: 0,
          candidateId: this.member?.id===0 ? 0 : this.member?.id,
          attachmentType: ['',Validators.required],
          fileName: ['', Validators.required],
          url: [{value:'',disabled: true}],
          attachmentSizeInBytes: 0
        })
      }

      addUserAttachment() {
        this.userAttachments.push(this.newUserAttachment());
      } 

      removeUserAttachment(i:number) {
        this.userAttachments.removeAt(i);
        this.userAttachments.markAsDirty();
        this.userAttachments.markAsTouched();
      }

    //validations  
      validateEmailNotTaken(): AsyncValidatorFn {
        return control => {
          return timer(10).pipe(
            switchMap(() => {
              if (!control.value) {
                return of(null);
              }
              return this.accountService.checkEmailExists(control.value).pipe(
                map(res => {
                  if (res !== null) this.toastr.warning('that email is taken by ' + res);
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
                  if (res !== null) this.toastr.warning('that passport number is taken by ' + res);
                  return res ? {ppExists: true} : null;
                })
              );
            })
          )
        }
      }

      fileToDataURL(file_: File) {
        const reader = new FileReader();
        // tslint:disable-next-line:no-unused
        return new Promise((resolve, reject) => {
          reader.readAsDataURL(file_);
          reader.addEventListener(
            'load',
            function (event) {
              resolve(event.target!.result);
            },
            false,
          );
        });
      }
    
      formReset() {
        //this.form.filesToUpload.reset();
        this.isMultipleUploaded = false;
        for (let i = 0; i < this.percentUploaded.length; i++) {
          this.percentUploaded[i] = 0;
        }
      }

      selectTab(tabId: number) {
          this.memberTabs!.tabs[tabId].active = true;
        }

      onTabActivated(data: TabDirective) {
        this.activeTab = data;
      }

      clearCategories() {
        this.form.get('userProfessions')!.patchValue([]);
      }
  
      closeForm() {

      }

      uploadFile = () => {
        if(this.form.invalid) {
          this.toastr.warning("Invalid form");
          return;
        }
       
        var microsecondsDiff: number= 28000;
        var nowDate: number =Date.now();
        
        //console.log('nowDate', nowDate, ' last time', this.lastTimeCalled);
        if(nowDate < this.lastTimeCalled+ microsecondsDiff) {
          //console.log('repeat call dialowed at', nowDate, ' last time called at', this.lastTimeCalled);
          return;
        }

        //console.log('upload file proceeded to api at ', nowDate, ', last time called: ', this.lastTimeCalled);

        this.lastTimeCalled=Date.now();
        
        const formData = new FormData();
        
        const mData = JSON.stringify(this.form.value);
        formData.append('data', mData);
        //console.log('formData with candidate object:', formData);
        
        if(this.userFiles.length > 0) {
          this.userFiles.forEach( f => {
            formData.append('file', f, f.name);
            //console.log('formData with candidate object + attachment:', formData);
          })
        }

        switch(this.imageFile!.type) {     //photo url of the candidate
            case 'image/jpeg':
            case 'image/jpg':
              formData.append('imageFile', this.imageFile!, 'imageFile.jpeg');
              break;
            case 'image.png':
              formData.append('imageFile', this.imageFile!, 'imageFile.png');
              break;
            default:
              break;
        }

        if(this.member?.id === 0) {   //insert new cv
            this.fileService.registerNewCandidate(formData).subscribe({
              next: (event) => 
              {
                if (event.type === HttpEventType.UploadProgress)
                  this.progress = Math.round(100 * event.loaded / event.total!);
                else if (event.type === HttpEventType.Response) 
                {
                    this.message = 'Upload success.';
                    this.form.get('applicationNo')!.setValue(event.body!.returnInt);
                    //console.log('response returned from api', event);
                    this.onUploadFinished.emit(event.body);
                    this.toastr.success('the candidate is inserted, and assigned application No. '+  event.body!.returnInt);
                } 
              },
            error: (err: HttpErrorResponse) => console.log(err)
            });
        } else {
            this.fileService.updateWithFiles(formData).subscribe({
              next: (event) => {
                if (event.type === HttpEventType.UploadProgress)
                  this.progress = Math.round(100 * event.loaded / event.total!);
                else if (event.type === HttpEventType.Response) {
                  this.message = 'Upload success.';
                  
                  this.toastr.success("Candidate Updated and Files uploaded");
                  this.onUploadFinished.emit(event.body);
                }
              },
            error: (err: HttpErrorResponse) => console.log(err)
            });
        }
      }

      uploadFinished = (event: any) => { 
        this.response = event; 
      }

      download (i: number) {
        //this.attachmentid = this.userAttachments['at'](i).get('id').value;
        console.log('userattachments', this.userAttachments);
        this.attachmentid = this.userAttachments.at(i).get('id')!.value;
        if(this.attachmentid===0) return;
        //var filename = this.userAttachments['at'](i).get('fileName').value;
        var filename = this.userAttachments.at(i).get('fileName')!.value;
          this.fileService.download(this.attachmentid).subscribe((event) => {
              if(event.type===HttpEventType.DownloadProgress)  
                  this.progress = Math.round((100 * event.loaded) / event.total!);
              else if(event.type===HttpEventType.Response) {
                  this.message='download success';
                  this.downloadFile(event, filename);
              }
          });
      }

      onPhotoChange(event: Event) {
        const target = event.target as HTMLInputElement;
        const files = target.files as FileList;
        const f = files[0];
        console.log('image size in bits',Math.round(f.size), 'type is:', f.type);
        
        if(!(f.type ==='jpeg' || f.type ==='jpg' || f.type==='png' || f.type==='image/jpeg' ) )  {
          this.toastr.warning('only JPEG, JPG or PNG files accepted for image');
          return;
        }
        this.imageFile=f;

        //this.form.get('photoUrl').setValue(f.name);
        //this.member.photoUrl=f.name;
    }

      onFileInputChange(event: Event) {
          const target = event.target as HTMLInputElement;
          const files = target.files as FileList;
          const f = files[0];
          
          var newAttachment =  this.fb.group({
              candidateId: this.member?.id===0 ? 0 : this.member?.id,
              attachmentType: '',
              fileName: this.member!.applicationNo ===0  ? f.name : this.member!.applicationNo + '-' + f.name,
              attachmentSizeInBytes: Math.round(f.size/1024)
            })
          this.userAttachments.push(newAttachment);
          this.userFiles.push(f);
      }
  
      private downloadFile = (data: HttpResponse<Blob>, filename: string) => {
        const downloadedFile = new Blob([data.body!], { type: data.body!.type });
        const a = document.createElement('a');
        a.setAttribute('style', 'display:none;');
        document.body.appendChild(a);
        a.download = filename;
        a.href = URL.createObjectURL(downloadedFile);
        a.target = '_blank';
        a.click();
        document.body.removeChild(a);
      }
      

}
