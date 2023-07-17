import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IDeploymentDto } from 'src/app/shared/models/process/deploymentdto';
import { DeployService } from '../deploy.service';
import { IDeployStage } from 'src/app/shared/models/masters/deployStage';

@Component({
  selector: 'app-dep-modal',
  templateUrl: './dep-modal.component.html',
  styleUrls: ['./dep-modal.component.css']
})
export class DepModalComponent implements OnInit {

  @Input() emitObj = new EventEmitter();
  
  //from calling program
  dep: IDeploymentDto[] = [];
  depStatuses: IDeployStage[]=[];

  cvrefid: number=0;
  candidatename: string='';
  companyName: string='';
  applicationNo: number=0;
  categoryRef: string='';
  selectedAs: string='';

  //end of variables from calng program

  form: FormGroup = new FormGroup({});

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder, 
    private changeDetectorRef: ChangeDetectorRef
    , private service: DeployService) { }

  ngOnInit(): void {
    this.createForm();
    this.editForm();
    //this.getDeploymentDto(this.cvrefid);
    this.changeDetectorRef.detectChanges();
  }

  createForm() {
    this.form = this.fb.group({
      candidateName: '', 
      applicationNo: 0, 
      categoryRef: '', 
      categoryName: '', 
      depItems: this.fb.array([])
    })
  }

  editForm() {
    
    this.form.patchValue({
      candidateName: this.candidatename, applicationNo: this.applicationNo,
      categoryRef: this.categoryRef, categoryName: this.selectedAs
    })

    this.form.setControl('depItems', this.setDepItems(this.dep));
  }

  setDepItems(items: IDeploymentDto[]): FormArray {
    const formArray = new FormArray([]);
    items.forEach(i => {
      formArray.push(this.fb.group({
        transactionDate: i.transactionDate, 
        sequence: i.sequence, 
        nextSequence: i.nextSequence,
        nextStageDate: i.nextStageDate,
        cvRefId: i.deployCVRefId, 
        id: i.id
      }))
    });
    
    return formArray;
  }

  get depItems(): FormArray{
    return this.form.get('depItems') as FormArray;
  }

  getNextSequence(seq: number) {

    if(seq ===0) seq= this.findMaxSeq();

    var nxt = this.depStatuses.find(x => x.sequence == seq)?.nextSequence;
    return nxt;    
  }
  
  findMaxSeq(){
    //first row has the max seq, by design
    var t= this.depItems.value[0].sequence;
    
    return t;
  }

  getNextStageDate(id: number) {
    var lastDt: Date = new Date(this.depItems.value[0].transactionDate);
    
    var days = this.depStatuses.find(x => x.sequence==id)?.estimatedDaysToCompleteThisStage;
    
    return new Date(lastDt.setDate(lastDt.getDate() + days!));

  }

  
  newItem(): FormGroup {
    var sequence = this.getNextSequence(0);
    var nextSequence=this.getNextSequence(sequence!);
    var nextdt = this.getNextStageDate(nextSequence!)

    return this.fb.group({
      //transactionDate: Date(), stageId: this.getNextStageId
      transactionDate: new Date(),
      sequence: [sequence, Validators.required],
      nextSequence: nextSequence,
      nextStageDate: nextdt
    });
  }

  addItem() {
    this.depItems.push(this.newItem());
  }

  removeItem(i: number) {
    this.depItems.removeAt(i);
    this.depItems.markAsDirty();
    this.depItems.markAsTouched();
  }

  onSubmit() {
      this.service.updateDeployment(this.depItems.value).subscribe(response => {
        console.log('udated deployment from modal');
      });
      this.emitObj.emit(this.depItems.value);
      this.bsModalRef.hide();
    
  }

}
