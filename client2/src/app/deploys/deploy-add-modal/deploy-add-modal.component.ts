import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { DeployService } from '../deploy.service';
import { IDeployStage } from 'src/app/shared/models/masters/deployStage';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { CVReferredDto, ICVReferredDto, IDeployDto } from 'src/app/shared/dtos/admin/cvReferredDto';
import { Deployment, IDeployment } from 'src/app/shared/models/process/deployment';
import { CVRefDto, ICVRefDto } from 'src/app/shared/dtos/admin/cvRefDto';

@Component({
  selector: 'app-deploy-add-modal',
  templateUrl: './deploy-add-modal.component.html',
  styleUrls: ['./deploy-add-modal.component.css']
})
export class DeployAddModalComponent implements OnInit {
 
  @Input() EditEvent = new EventEmitter();

  ref?: ICVReferredDto;
  dep: IDeployment[]=[];

  deployStatuses: IDeployStage[]=[];
  //bsValueDate = new Date();

  form: FormGroup = new FormGroup({});

  constructor(
    private fb: FormBuilder
    , public bsModalRef: BsModalRef
    , private toastr: ToastrService
    , private service: DeployService 
    
    ) { }

  ngOnInit(): void {
    /*this.service.getDeployStatus().subscribe(response => {
      this.depStatuses = response;
    });
    */
    this.dep = this.ref?.deployments!;
    this.createForm();
    this.editDep(this.ref!);
  }

  createForm() {
    this.form = this.fb.group({
        cvRefId: [0],
        customerName: '',
        orderNo: 0,
        orderDate: '',
        categoryName: '',
        categoryRef: '',
        applicationNo: 0,
        candidateName: '',
        selectedOn: '',
        deployments: this.fb.array([])
    })
  }

  editDep(cv: ICVReferredDto) {
    this.form.patchValue({
      cvRefId: cv.cvRefId, customerName: cv.customerName, orderNo: cv.orderNo,
      orderDate: cv.orderDate, categoryName: cv.categoryName, categoryRef: cv.categoryRef,
      applicationNo: cv.applicationNo, candidateName: cv.candidateName,
      selectedOn: cv.selectedOn
    });

    this.form.setControl('deployments', this.setExistingItems( cv.deployments ));
  }

  setExistingItems(items: IDeployDto[]): FormArray {
    const formArray = new FormArray([]);
    items.forEach(it => {
      formArray.push(this.fb.group({
        id: it.id, deployCVRefId: it.deployCVRefId, transactionDate: it.transactionDate,
        sequence: it.sequence, nextSequence: it.nextSequence, 
        nextStageDate: it.nextStageDate
      }))
    });
    return formArray;
  }
  
  update() {

    //this.EditEvent.emit(this.form.value);
    //this.bsModalRef.hide();
    

    var model = this.form.value;
    var dep= this.form.get('deployments')?.value;
      this.service.updateDeployment(dep).subscribe({
        next: (success: boolean) => {
          console.log('deploy add modal edit event, subscribed value:', success);
          this.EditEvent.emit(success);
          this.bsModalRef.hide();
        },
        error: err => {
          this.EditEvent.emit(false);
          this.toastr.error('failed to update the record', err);
        }
      });
    
  }

  close() {
    this.EditEvent.emit(false);
    this.bsModalRef.hide();
  }

  get deployments(): FormArray {
      return this.form.get('deployments') as FormArray;
  }

  newItem(): FormGroup{
    var seq = this.getSequenceForNextTransaction();
    var dt = new Date();

    console.log('ref:', this.ref);

      var cvrefid = this.ref===null ? 0 : this.ref?.cvRefId;

      return this.fb.group({
        id:0, 
        cVRefId: cvrefid, 
        sequence: seq,
        nextSequence: this.getNextSequenceInDepStatuses(seq),
        transactionDate: dt,
        nextStageDate: this.getNextStageDateForNextTransaction(dt,seq)
        
      })
  
  }

  addItem() {
    this.deployments.push(this.newItem());
  }

  removeItem(i: number) {
    this.deployments.removeAt(i);
    this.deployments.markAsDirty();
    this.deployments.markAsTouched();
  }

  getSequenceForNextTransaction(): number {
  
    var currentSeq = this.findMaxSeq();

    var seqForNextTrasaction = this.deployStatuses.find(x => x.sequence == currentSeq)?.nextSequence;
    return seqForNextTrasaction ?? 0;    
  }
  
  getNextSequenceInDepStatuses(seq: number): number{
    var next = this.deployStatuses.find(x => x.sequence==  seq)?.nextSequence ?? 0;
    return next;
  }
  
  
  findMaxSeq(){
    var t= Math.max(...this.deployments.value.map((x:any) => x.sequence));
    return t;
  }

  getNextStageDateForNextTransaction(nextdt: Date, nextSeq: number) {
    
    var days = this.deployStatuses.find(x => x.sequence==  nextSeq)?.estimatedDaysToCompleteThisStage || 0;

    return new Date(nextdt.setDate(nextdt.getDate() + days));

  }

}
