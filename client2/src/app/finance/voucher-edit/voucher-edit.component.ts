import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICOA } from 'src/app/shared/models/finance/coa';
import { FinanceVoucher, IFinanceVoucher } from 'src/app/shared/models/finance/financeVoucher';
import { IVoucherAttachment, VoucherAttachment } from 'src/app/shared/models/finance/voucherAttachment';
import { IVoucherEntry, VoucherEntry } from 'src/app/shared/models/finance/voucherEntry';
import { VouchersService } from '../vouchers.service';
import { CoaService } from '../coa.service';
import { FileService } from 'src/app/shared/services/file.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { catchError, filter, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { CoaEditModalComponent } from '../coa-edit-modal/coa-edit-modal.component';
import { IVoucherToAddDto, voucherToAddDto } from 'src/app/shared/dtos/finance/voucherToAddDto';
import { HttpErrorResponse, HttpEventType, HttpResponse } from '@angular/common/http';
import { IApiReturnDto } from 'src/app/shared/dtos/admin/apiReturnDto';
import { IEmployeeIdAndKnownAs } from 'src/app/shared/models/admin/employeeIdAndKnownAs';

@Component({
  selector: 'app-voucher-edit',
  templateUrl: './voucher-edit.component.html',
  styleUrls: ['./voucher-edit.component.css']
})
export class VoucherEditComponent implements OnInit {

  @ViewChild('editForm') editForm?: NgForm;
  //@Output() public onUploadFinished = new EventEmitter();

  voucher?: IFinanceVoucher;
  entries: IVoucherEntry[]=[];

  coas: ICOA[]=[];
  emps: IEmployeeIdAndKnownAs[]=[];
  user?: IUser;
  
  selectedAccountId: number=0;
  
  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  minTransDate = new Date();
  maxTransDate = new Date();

  returnUrl: string='';
  routeId: string='';
  routeResumeId: string='';


  isAddMode: boolean = false;
  isEditable: boolean = false;

  loading = false;
  submitted = false;
  bolNavigationExtras:boolean=false;

  totalAmountDR: number = 0;
  totalAmountCR: number = 0;
  voucherAmount = 0;

  diff: string='';

  form: FormGroup = new FormGroup({});

  bsModalRef?: BsModalRef;

  suggestedDefaultCoaIdDR: number=0;
  suggestedDefaultCoaIdCR: number=0;
  suggestedDefaultAmountDR: number=0;
  suggestedDefaultAmountCR: number=0;

  //fileupload variables
  attachmentid: number=0;
  fileAttachments: IVoucherAttachment[]=[];
  voucherFiles: File[] = [];
  
  public progress: number=0;
  public message: string='';  
  isMultipleUploaded = false;
  isSingleUploaded = false;
  urlAfterUpload = '';
  percentUploaded = [0];
  lastTimeCalled: number= Date.now();
  
  //filedownoads
  response?: {dbPath: ''};
  
  constructor(private service: VouchersService, 
    private coaService: CoaService,
    private fileService: FileService,
    private bcService: BreadcrumbService, 
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    //private accountService: AccountService, 
    private toastr: ToastrService, 
    private fb: FormBuilder,
    private modalService: BsModalService,
    private confirmService: ConfirmService) { 
        //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
          
        bcService.set('@Edit Finance Voucher', ' ');

        this.routeId = this.activatedRoute.snapshot.params['id'];
        if(this.routeId==undefined) this.routeId='';
        if(this.routeId==='' 
          //&&  (this.voucher===null || this.voucher===undefined)
        ) this.isAddMode=true;

        //read navigationExtras
        let nav: Navigation | null= this.router.getCurrentNavigation();
          
        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state.userObject) this.user = nav.extras.state.userObject;
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            this.isEditable = nav.extras.state.toedit || this.isAddMode;
        }
          
        this.minDate.setDate(this.minDate.getDate() - 365);
        this.maxDate.setDate(this.maxDate.getDate() + 365);
        this.minTransDate.setDate(this.minTransDate.getDate()-365);   //max o2 day back
        this.bcService.set('@FinanceTransaction',' ');
    }

    ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
        this.coas = data.coas;
        this.voucher = data.voucher;
        
      })

      if(this.voucher===undefined) this.voucher=new FinanceVoucher();
      
      this.createForm();
      if(!this.isAddMode) this.editVoucher(this.voucher);
      if(!this.isEditable) this.form.disable();
      
      this.recalculateTotal();
    }

    getVoucher(routeId: number) {
      this.service.getVoucherFromId(routeId).subscribe((response: any) => {
        this.voucher = response;
      })
    }

    createForm() {
      this.form = this.fb.group({
        id: 0,
        divn: ['', Validators.maxLength(1)],
        voucherNo: [0, Validators.required],
        voucherDated: ['', Validators.required],
        coaId: [0, Validators.required],
        amount: [0, Validators.required],
        narration: '',
        employeeId: 0,
        voucherEntries: this.fb.array([]),
        voucherAttachments: this.fb.array([])
      })
    }

    editVoucher(v: IFinanceVoucher) {
      this.form.patchValue( {
        id: v.id, 
        divn: v.divn, 
        voucherNo: v.voucherNo, 
        voucherDated: v.voucherDated, 
        coaId: v.coaId,
        amount: v.amount, 
        narration : v.narration, 
        employeeId: v.employeeId,
      });

      if(v.voucherEntries !== null && v.voucherEntries !== undefined) 
        this.form.setControl('voucherEntries', this.setExistingVoucherEntries(v.voucherEntries) );
      
        
      if(v.voucherAttachments !== null && v.voucherAttachments !== undefined) 
          this.form.setControl('voucherAttachments', this.setExistingVoucherAttachments(v.voucherAttachments));
      
    }


  //voucher entries
    setExistingVoucherEntries(e: IVoucherEntry[]): FormArray {
      
      const formArray = new FormArray([]);
      e.forEach(x => {
        formArray.push(this.fb.group({
          id: x.id, financeVoucherId: x.financeVoucherId, coaId: x.coaId, 
          accountName: x.accountName, transDate: x.transDate, 
          dr: x.dr, cr: x.cr, narration: x.narration,
          approved: x.drEntryApproved, approvedOn: x.drEntryApprovedOn,
          approvedByEmployeeId: x.drEntryApprovedByEmployeeId
        }))
      });
      return formArray;
    }

    get voucherEntries(): FormArray {
      return this.form.get("voucherEntries") as FormArray;
    }
    
    addVoucherEntry() {
      if(this.voucherEntries.length==0) { 
          this.setDefaultEntryValues();
          this.voucherEntries.push(this.newDRVoucherEntry());
          this.voucherEntries.push(this.newCRVoucherEntry());
      } else {
        var diff = this.totalAmountDR - this.totalAmountCR;
        if(diff < 0) {    //CREDIT
          this.suggestedDefaultAmountDR = Math.abs(diff);
          this.voucherEntries.push(this.newDRVoucherEntry());
        } else {
          this.suggestedDefaultAmountCR = Math.abs(diff);
          this.suggestedDefaultCoaIdCR = this.form.controls['coa'].value;

          this.voucherEntries.push(this.newCRVoucherEntry());
        }
      }
      this.recalculateTotal();
    }
    
    newDRVoucherEntry(): any {
      return this.fb.group({
        id: 0,
        financeVoucherId: this.voucher?.id === undefined ? 0 : this.voucher.id, 
        transDate: new Date(), 
        coaId: this.suggestedDefaultCoaIdDR, 
        accountName:'', 
        dr: this.suggestedDefaultAmountDR, 
        cr: 0,
        narration: '',
        drEntryApproved: false,
        //approved: [false, {disabled:!this.user?.roles.includes('finance')}],
        drEntryApprovedOn: new Date(),
        drEntryApprovedByEmployeeId: this.user?.loggedInEmployeeId,
        filePath: '',
        fileName:'',
        fileType: '',
        fileSize:0
      })
    }

    newCRVoucherEntry(): any {
      var craccountid=this.form.controls['coaId'].value;
      return this.fb.group({
        id: 0,
        financeVoucherId: this.voucher?.id === undefined ? 0 : this.voucher.id, 
        transDate: new Date(), 
        coaId: craccountid ?? this.suggestedDefaultCoaIdCR, 
        accountName:'', 
        cr: this.suggestedDefaultAmountCR ,
        dr: 0,
        narration: '',
        filePath: '',
        fileName:'',
        fileType: '',
        fileSize:0
      })
    }
    
    removeVoucherEntry(i: number) {
      this.voucherEntries.removeAt(i);
      this.voucherEntries.markAsDirty();
      this.voucherEntries.markAsTouched();
    } 

//voucher attachments
    setExistingVoucherAttachments(v: IVoucherAttachment[]): FormArray {
      const formArray = new FormArray([]);
      v.forEach(x => {
        formArray.push(this.fb.group({
          id: x.id,
          voucherId: x.voucherId,
          fileName: x.fileName,
          url: x.url,
          uploadedByEmployeeId: x.uploadedByEmployeeId,
          dateUploaded: x.dateUploaded,
          attachmentSizeInBytes: x.attachmentSizeInBytes
        }))
      });
      console.log('in setEistingvcherattachments, frmArray is:', formArray);
      return formArray;
    }

    newVoucherAttachment(): any {
      return this.fb.group({
        id: 0, 
        voucherId: this.voucher?.id ?? 0,
        fileName: ['', Validators.required],
        url: [{value:'',disabled: true}],
        attachmentSizInBytes: 0,
        dateUploaded: new Date(),
        uploadedByEmployeeId: 0
      })

      
    }

    addVoucherAttachmnt() {
        this.voucherAttachments.push(this.newVoucherAttachment());
    }

    removeVoucherAttachment(i: number) {
      this.voucherAttachments.removeAt(i);
      this.voucherAttachments.markAsDirty();
      this.voucherAttachments.markAsTouched();
     } 

    get voucherAttachments(): FormArray {
      return this.form.get("voucherAttachments") as FormArray;
    }

    
    recalculateTotal() {
      this.totalAmountDR = +this.voucherEntries.value.map((x: any) => x.dr).reduce((a:number,b:number) => a+b,0);
      this.totalAmountCR = +this.voucherEntries.value.map((x: any) => x.cr).reduce((a:number,b:number) => a+b,0);
      var d = this.totalAmountDR - this.totalAmountCR;
      this.diff = Math.abs(d).toString();
      this.diff += d > 0 ? ' DR' : ' CR';
    }

    updateVoucherAmount()
    {
        this.voucherAmount = this.form.controls['amount'].value;
    }

    setDefaultEntryValues(): any {
      var icoaId = this.form.controls['coaId'].value;
      var vAmount = +this.form.controls['amount'].value;

      var accountclass = this.coas.filter(x => x.id == icoaId).map(x => x.accountClass);
      
      this.suggestedDefaultAmountCR = vAmount; //petty cash
      this.suggestedDefaultAmountDR = vAmount;
      
      switch(accountclass[0]) {
        case 'exp':  //refeshment
          this.suggestedDefaultCoaIdCR = this.suggestedDefaultAmountCR > 5000 ? 14: 2;  //Bank or petty cash
          this.suggestedDefaultCoaIdDR =  icoaId;
          break;

        case 'salary':   //salary
        case 'businessvisit': //business visits
          this.suggestedDefaultCoaIdDR = icoaId;
          this.suggestedDefaultCoaIdCR =  14;  //Afreen kapur c/a
          break;
          break;
        
        case 'candidate':
          this.suggestedDefaultCoaIdDR = icoaId;
          this.suggestedDefaultCoaIdCR = 20;    //sales recruitment
          break;

        default:
          break;
      }

    }

    deleteVoucher() {
      this.confirmService.confirm('confirm delete this voucher', 'confirm delete voucher').pipe(
        filter(confirm => !confirm),
        switchMap(() => this.service.deleteVoucher(this.voucher?.id!).pipe(

          /*catchError((err: any) => {
            //console.log('Error in deleting the voucher', err);
            return of(undefined);
          })
          
          ,*/
          tap((res: boolean) => this.toastr.success('deleted voucher')),
          //tap(res=>console.log('delete voucher succeeded')),
        )),
        catchError(err => {
          this.toastr.error('Error in getting delete confirmation', err);
          return of();
        })
      ).subscribe(
        deleteReponse => {
          console.log('delete succeeded');
          this.toastr.success('voucher deleted');
        },
        err => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
        }
      )
        /*
        this.service.deleteVoucher(this.voucher.id).subscribe(response  => {
          if (response) {
            this.toastr.success('Voucher deleted');
            this.returnToCaller();
          } else {
            this.toastr.error('failed to delete the voucher');
          }
        }, error => {
          this.toastr.error('failed to delete the Voucher', error);
        })
        */
    }


    returnToCaller() {
      this.router.navigateByUrl(this.returnUrl || '');
    }

    addNewCOA() {

      const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          title: 'add a new Chart Of Account',
          accountType: '',
          accountName: '',
          divn: '',
          accountClass: '',
          opBalance: 0
        }
      };

      this.bsModalRef = this.modalService.show(CoaEditModalComponent, config);
      // First API call
      this.bsModalRef.content.editCOAEvent
      .pipe(
          // Logging has a side effect, so use tap for that
          //tap(icoa => console.log('First API success', icoa)),
          // Call the second if first succeeds
          switchMap(coaObj => this.coaService.addNewCOA(coaObj).pipe(
              // Handle your error for second call, return of(val) if succeeds or throwError() if you can't handle it here
              catchError(err => { 
                  console.log('Error for coaService.AddNewCOA', err);
                  return of(); 
              }),
              // Logging is a side effect, so use tap for that
              //tap(res => console.log('Second API success', res)),
              //update DOM
              
          )),
          // Handle your error for first call, return of(val) if succeeds or throwError() if you can't handle it here
          catchError(err => { 
            //console.log('Error for first API- ICOA objct frm odal form)', err);
            return of(); 
          }) 
      ).subscribe(
          // handle both successfull
          (coaAdded : any) => {
            this.coas.push(coaAdded)
          //console.log('Both APIs succeeded, result from 2) is returned', coaAdded);
        }),
        // handle uncaught errors
        (err : any) => {
          console.log('Any error NOT handled in catchError() or if you returned a throwError() instead of of() inside your catchError()', err);
        } 
    }

    onFileInputChange(event: Event, voucherId: number) {
      const target = event.target as HTMLInputElement;
      const files = target.files as FileList;
      const f = files[0];
      
      var newAttachment =  new VoucherAttachment();
      newAttachment.voucherId=voucherId ?? 0;
      newAttachment.fileName= f.name;
      newAttachment.attachmentSizeInBytes= Math.round(f.size/1024)
      newAttachment.dateUploaded=new Date;      
      this.fileAttachments.push(newAttachment);
      this.voucherFiles.push(f);

      //add to the formArray

      var newFileAttachment =  this.fb.group({
        voucherId: this.voucher?.id ?? 0,
        fileName: f.name,
        attachmentSizeInBytes: Math.round(f.size/1024)
      })
      this.voucherAttachments.push(newFileAttachment);

    }

    uploadFileAndFormData = () => {
      if(this.form.invalid) {
        this.toastr.warning("Invalid form");
        return;
      }
     
      var microsecondsDiff: number= 28000;
      var nowDate: number =Date.now();
      
      //console.log('nowDate', nowDate, ' last time', this.lastTimeCalled);
      if(nowDate < this.lastTimeCalled+ microsecondsDiff) {
        console.log('repeat call dialowed at', nowDate, ' last time called at', this.lastTimeCalled);
        return;
      }

      //console.log('upload file proceeded to api at ', nowDate, ', last time called: ', this.lastTimeCalled);

      this.lastTimeCalled=Date.now();
      
      const formData = new FormData();
      const formValue = this.form.value;

      //const mData = JSON.stringify(this.form.value);
      //formData.append('data', mData);
      //console.log('formData with candidate object:', formData);
      
      if(this.voucherFiles.length > 0) {
        this.voucherFiles.forEach( f => {
          formData.append('file', f, f.name);
          //console.log('formData with candidate object + attachment:', formData);
        })
      }


      if(this.voucher!.id === 0) {   //insert new cv
        var newVoucher = new voucherToAddDto();

        var voucherentries: VoucherEntry[]=[];
        voucherentries = this.voucherEntries.value;
        newVoucher.divn= formValue.divn;
        newVoucher.voucherDated= this.form.controls['voucherDated'].value;  // new Date(nowDate);
        newVoucher.amount=formValue.amount;
        newVoucher.narration=formValue.narration;
        newVoucher.voucherEntries=voucherentries;
        newVoucher.coaId = this.form.controls['coaId'].value;
        
        formData.append('data', JSON.stringify(newVoucher));


        this.service.insertVoucherWithUploads(formData).subscribe({
            next: (event: any) => 
            {
              if (event.type === HttpEventType.UploadProgress)
                this.progress = Math.round(100 * event.loaded / event.total);
              else if (event.type === HttpEventType.Response) 
              {
                  this.message = 'Upload success.';
                  console.log('event', event);

                  this.form.get('voucherNo')?.setValue(event.body.returnInt);
                  //console.log('response returned from api', event);
                  //this.onUploadFinished.emit(event.body);
                  this.returnToCaller();
                  this.toastr.success('the Voucher is created, and assigned Voucher No. '+  event.body.returnInt);
              } 
            },
          error: (err: HttpErrorResponse) => console.log(err)
          });

      } else {

          formData.append('data', JSON.stringify(this.form.value));
          console.log('sent to api formdata:', formData);

          this.service.updateWithFiles(formData).subscribe({
            next: (event: any) => {
              if (event.type === HttpEventType.UploadProgress)
                this.progress = Math.round(100 * event.loaded / event.total);
              else if (event.type === HttpEventType.Response) {
                this.message = 'Upload success.';
                
                this.toastr.success("Candidate Updated and Files uploaded");
                //this.onUploadFinished.emit(event.body);
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
      this.attachmentid = this.voucherAttachments.at(i).get('id')?.value;
      if(this.attachmentid===0) return;
      //var filename = this.userAttachments['at'](i).get('fileName').value;
      var filename = this.voucherAttachments.at(i).get('fileName')?.value;
        this.fileService.download(this.attachmentid).subscribe((event: any) => {
            if(event.type===HttpEventType.DownloadProgress)  
                this.progress = Math.round((100 * event.loaded) / event.total);
            else if(event.type===HttpEventType.Response) {
                this.message='download success';
                this.downloadFile(event, filename);
            }
        });
    }

    private downloadFile = (data: HttpResponse<Blob>, filename: string) => {
      const downloadedFile = new Blob([data.body!], { type: data.body?.type });
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
