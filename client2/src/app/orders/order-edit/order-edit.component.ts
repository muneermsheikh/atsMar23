import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IJDDto } from 'src/app/shared/dtos/admin/jdDto';
import { IRemunerationDto } from 'src/app/shared/dtos/admin/remunerationDto';
import { IContractReview } from 'src/app/shared/models/admin/contractReview';
import { IContractReviewItem } from 'src/app/shared/models/admin/contractReviewItem';
import { ICustomerOfficialDto } from 'src/app/shared/models/admin/customerOfficialDto';
import { ICustomerNameAndCity } from 'src/app/shared/models/admin/customernameandcity';
import { IEmployeeIdAndKnownAs } from 'src/app/shared/models/admin/employeeIdAndKnownAs';
import { IOrder, Order } from 'src/app/shared/models/admin/order';
import { IReviewItemStatus } from 'src/app/shared/models/admin/reviewItemStatus';
import { IUser } from 'src/app/shared/models/admin/user';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { OrderService } from '../order.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { ReviewService } from '../review.service';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { ToastrService } from 'ngx-toastr';
import { UserTaskService } from 'src/app/userTask/user-task.service';
import { DlForwardService } from '../dl-forward.service';
import { IOrderItem } from 'src/app/shared/models/admin/orderItem';
import { ContractReviewItemModalComponent } from '../contract-review-item-modal/contract-review-item-modal.component';
import { IJobDescription } from 'src/app/shared/models/admin/jobDescription';
import { JdModalComponent } from '../jd-modal/jd-modal.component';
import { RemunerationModalComponent } from '../remuneration-modal/remuneration-modal.component';
import { IRemuneration } from 'src/app/shared/models/admin/remuneration';
import { ApplicationTask } from 'src/app/shared/models/admin/applicationTask';
import { ChooseAgentsModalComponent } from '../choose-agents-modal/choose-agents-modal.component';
import { dLForwardToAgent } from 'src/app/shared/models/admin/dlforwardToAgent';
import { IDLForwardCategory, dLForwardCategory } from 'src/app/shared/models/admin/dlForwardCategory';
import { dLForwardCategoryOfficial } from 'src/app/shared/models/admin/dlForwardCategoryOfficial';
import { IOrderAssignmentDto, orderAssignmentDto } from 'src/app/shared/dtos/admin/orderAssignmentDto';

@Component({
  selector: 'app-order-edit',
  templateUrl: './order-edit.component.html',
  styleUrls: ['./order-edit.component.css']
})
export class OrderEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm | undefined;

  routeId: string;

  member?: IOrder;
  user?: IUser;
  
  form: FormGroup = new FormGroup({});

  selectedCategoryIds: number[]=[];
  categories: IProfession[]=[];
  employees: IEmployeeIdAndKnownAs[]=[];
  customers: ICustomerNameAndCity[]=[];
  associates: ICustomerOfficialDto[]=[];
  
  fileToUpload: File | null = null;

  events: Event[] = [];

  isAddMode: boolean = false;
  loading = false;
  submitted = false;

  errors: string[]=[];

  bsValue = new Date();
  bsRangeValue = new Date();
  maxDate = new Date();
  minDate = new Date();

  //file uploads
  uploadProgress = 0;
  selectedFiles: File[]=[];
  uploading = false;
  fileErrorMsg = '';

  bsValueDate = new Date();
  bsModalRef: BsModalRef | undefined;
  jd?: IJDDto;
  remun?: IRemunerationDto;
  cReview?: IContractReview;
  //itemReviews: IReviewItem[];
  contractReviewItem?: IContractReviewItem;
  
  mySelect = 0;
  selectedCustomerName: any;
  selectedProjManagerName: any;

  hasEditRole=false;
  hasHRRole=false;

  bolNavigationExtras: boolean=false;
  returnUrl: string='';

  orderReviewItemStatus: IReviewItemStatus[]=[];

  //modal choose agents
  existingOfficialIds: ICustomerOfficialDto[]=[]; // IChooseAgentDto[]=[];

  constructor(private service: OrderService, 
      private bcService: BreadcrumbService, 
      private dlforwardService: DlForwardService,
      private modalService: BsModalService,
      private activatedRoute: ActivatedRoute, 
      private router: Router, 
      private sharedService: SharedService, 
      private rvwService: ReviewService,
      private confirmService: ConfirmService,
      private toastr: ToastrService, 
      private taskService: UserTaskService,
      private fb: FormBuilder) {
          this.routeId = this.activatedRoute.snapshot.params['id'];
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              this.bolNavigationExtras=true;
              if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

              if( nav.extras.state.tasktoedit) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
          }
          this.bcService.set('@editOrder',' ');
   }

    ngOnInit(): void {

      this.createForm();

      this.activatedRoute.data.subscribe(data => { 
        this.member = data.order;
        this.associates = data.associates;
        this.customers = data.customers;
        this.employees = data.employees;
        this.categories = data.professions;
        if(this.member !== null && this.member !== undefined) this.editOrder(this.member);
      })

        this.isAddMode = !this.routeId;
        if(this.isAddMode)  this.member=new Order();
        if(this.member === undefined) {
          this.toastr.error('failed to retrieve order data');
        } 
        
    }

    createForm() {
        this.form = this.fb.group({
          id: [null],  orderNo: 0, orderDate: [new Date, Validators.required],
          customerId: [0, Validators.required], orderRef: '', salesmanId: 0, projectManagerId: [0, Validators.required],
          medicalProcessInchargeEmpId: 0, visaProcessInchargeEmpId: 0, emigProcessInchargeId: 0,
          travelProcessInchargeId: 0, cityOfWorking: '', country: '', completeBy: ['', Validators.required],
          status: 'Not Started', forwardedToHRDeptOn: '', contractReviewStatusId:0,
          orderItems: this.fb.array([])
        } //, {validator: MustMatch('password', 'confirmPassword')}
        );

      }

    editOrder(order: IOrder) {
      this.form.patchValue( {
        id: order.id, orderNo: order.orderNo, orderDate: order.orderDate, customerId: order.customerId,
        orderRef: order.orderRef, salesmanId: order.salesmanId, projectManagerId: order.projectManagerId, 
        medicalProcessInchargeId: order.medicalProcessInchargeEmpId, visaProcessInchargeEmpId: order.visaProcessInchargeEmpId,
        emigProcessInchargeId: order.emigProcessInchargeId, travelProcessInchargeId: order.travelProcessInchargeId,
        cityOfWorking: order.cityOfWorking, country: order.country, completeBy: order.completeBy,
        status: order.status, forwardedToHRDeptOn: order.forwardedToHRDeptOn, contractReviewStatusId:order.contractReviewStatusId
      });

      if (order.orderItems !== null) this.form.setControl('orderItems', this.setExistingItems(order.orderItems));
    }
    
    setExistingItems(items: IOrderItem[]): FormArray {
        const formArray = new FormArray([]);
        items.forEach(ph => {
          formArray.push(this.fb.group({
            selected: false, 
            id: ph.id, orderId: ph.orderId, srNo: ph.srNo, categoryId: ph.categoryId,
            ecnr: ph.ecnr, isProcessingOnly: ph.isProcessingOnly, industryId: ph.industryId,
            sourceFrom: ph.sourceFrom, quantity: ph.quantity, minCVs: ph.minCVs, maxCVs: ph.maxCVs,
            requireInternalReview: ph.requireInternalReview, requireAssess: ph.requireAssess,
            completeBefore: ph.completeBefore, hrExecId: ph.hrExecId, hrSupId: ph.hrSupId,
            hrmId: ph.hrmId, charges: ph.charges, feeFromClientINR: ph.feeFromClientINR, status: ph.status,
            reviewItemStatusId: ph.reviewItemStatusId, noReviewBySupervisor:ph.noReviewBySupervisor
          }))
        });
        return formArray;
    }

    get orderItems() : FormArray {
      return this.form.get("orderItems") as FormArray
    }

    newItem(): FormGroup {
      //get max SrNo
      
      var maxSrNo = this.orderItems.length===0 ? 1 : Math.max(...this.orderItems.value.map((x:any) => x.srNo))+1;

      return this.fb.group({
        selected: false, 
        id: 0, orderId: 0, srNo: maxSrNo, categoryId: [0,[Validators.required, Validators.min(1)]], ecnr: false, isProcessingOnly: false, industryId: 0,
        sourceFrom: 'India', quantity: [0, Validators.min(1)], minCVs: 0, maxCVs: 0, requireInternalReview: false, requireAssess: false,
        completeBefore: ['', Validators.required], hrExecId: 0, hrSupId: 0, hrmId: 0, charges: 0, feeFromClientINR: 0, status: 'Not Started',
        reviewItemStatusId: 0, noReviewBySupervisor: false })
    }

    addItem() {
      this.orderItems.push(this.newItem());
      //this.addCheckboxes();
    }

    removeItem(i:number) {
      this.orderItems.removeAt(i);
      this.orderItems.markAsDirty();
      this.orderItems.markAsTouched();
    }

     onSubmit() {
      if (this.isAddMode) {
          this.CreateOrder();
      } else {
          this.toastr.warning('updating order ...');
          this.UpdateOrder();
      }
    }

    private CreateOrder() {
      this.service.register(this.form.value).subscribe(response => {
        var order = response;
        this.confirmService.confirm('Create Acknowledgement message for client?', 
          'the DL is saved.  Do you want to compose the acknowledgement message to client?', 'Yes, compose', 'No, not now')
          .subscribe(result => {
          })
      }, error => {
        console.log(error);
        this.errors = error.errors;
      })
    }

    private UpdateOrder() {
      this.service.UpdateOrder(this.form.value).subscribe(response => {
        this.toastr.success('order updated');
        this.router.navigateByUrl('/orders');
      }, error => {
        console.log(error);
      })
    }

    handleFileInput(files: FileList) {
      this.fileToUpload = files.item(0);
    }

    getJD(orderitemid: number) {
      return this.service.getJD(orderitemid).subscribe(response => {
        this.jd=response;
      }, error => {
        this.toastr.error('failed to retrieve job description');
      })
    }

    getName(i: number) {
        return this.getControls()[i].value.id;
    }

    getSrNo(i: number) {
      return this.getControls()[i].value.srNo;
    }

    getQnty(index: number) {
      return this.getControls()[index].value.quantity;
    }
    getMinCVs(index: number) {
      return this.getControls()[index].value.minCVs;
    }

    setMinCVs(index: number, newValue: number) {
      this.orderItems!.at(index).get('minCVs')!.setValue(newValue);
    }
  
    getControls() {
      return (<FormArray>this.form.get('orderItems')).controls;
    }

    getContractReviewItem(orderitemid:number) {
      return this.rvwService.getReviewItem(orderitemid).subscribe(response => {
        this.contractReviewItem = response;
      }, error => {
        this.toastr.error('failed to retrieve review items', error);
      })
    }

    openReviewModal(index: number) {

      if(index===undefined) return;
      
      if(this.orderReviewItemStatus.length === 0) {
          this.rvwService.getReviewItemStatuses(true).subscribe(response => {
            this.orderReviewItemStatus = response;
            if (this.orderReviewItemStatus.length === 0) {
              this.toastr.warning('cannot continue, since failed to get Review Item status');
              return;
            }
          }, error => {
            this.toastr.error('cannot continue, since failed to get Review Item status', error);
            return;
          })
      }
      var orderitemid = this.getName(index);
        
        this.rvwService.getReviewItem(orderitemid).subscribe(response => 
          {
            this.contractReviewItem = response;
            const config = {
              class: 'modal-dialog-centered modal-lg',
              initialState: {
                review : this.contractReviewItem,
                reviewStatus: this.orderReviewItemStatus
              }
            };

            this.bsModalRef = this.modalService.show(ContractReviewItemModalComponent, config);
            
            this.bsModalRef.content.updateModalReview.subscribe((values: IContractReviewItem) => {
                  this.rvwService.updateReviewItem(values).subscribe((response)  => {
                    //returns IContractReviewItemReturnValueDto - reviewItemStatusId: number,orderReviewStatus: number
                    var dto =response;
                    if(dto !== null) {
                      this.orderItems.at(index).get('reviewItemStatusId')!.setValue (dto.reviewItemStatusId);
                      this.form.get('contractReviewStatusId')!.setValue(dto.orderReviewStatus);
                      this.toastr.success("Review Item updated");
                    } else {
                      this.toastr.error('failed to get return value from the api server');
                    }
                  }, error => {
                    this.toastr.error("failed to update the Reviews");
                  })
            }, (error: any) => {
              this.toastr.error('failed to call updateModalReview', error);
            })
          }, error => {
            this.toastr.error('failed to retrieve contract review item from api', error);
          })
    }

    openJDModal(index: number) {
      var orderitemid = this.getName(index);
        this.service.getJD(orderitemid).subscribe(response => {
            var jd = response;
            console.log('jd retrieved from api: ', jd);

            const initialState = {
              class: 'modal-dialog-centered modal-lg',
               title: 'job description',
               jd
            };
            this.bsModalRef = this.modalService.show(JdModalComponent, {initialState});
            //**TODO** IMPLEMENT SWITCHMAP HERE, TO AVOID SUBSCRIPTION NESTING - CHECK implementation in referral edit */
            this.bsModalRef.content.updateSelectedJD.subscribe((values: IJobDescription) => {
            this.service.updateJD(values).subscribe(() => {
              this.toastr.success("job description updated");
            }, error => {
              this.toastr.error("failed to update the job description");
            })
          }
          )
          
        }, error => {
          this.toastr.warning('failed to retrieve job description');
        })
        
    }

    openRemunerationModal(index: number) {
      var orderitemid = this.getName(index);
        this.service.getRemuneration(orderitemid).subscribe(response => {
            //this.remun=response;
            var remun = response;
            const initialState = {
              class: 'modal-dialog-centered modal-lg',
              remun
            };
            this.bsModalRef = this.modalService.show(RemunerationModalComponent, {initialState});
            
            this.bsModalRef.content.updateSelectedRemuneration.subscribe((values: IRemuneration) => {
            this.service.updateRemuneration(values).subscribe(() => {
              this.toastr.success("Remuneration updated");
            }, error => {
              this.toastr.error("failed to update the Remuneration");
            })
          }
          )
          
        }, error => {
          this.toastr.warning('failed to retrieve Remuneration data');
        })
        
    }

    forwardDLToHRDept() {

      if(this.member === null || this.member === undefined) return;

      if (this.member.forwardedToHRDeptOn!.getFullYear() > 2000) {
        this.toastr.warning('this DL has been forwarded earlier on ' + this.forwardDLToHRDept);
        return;
      }
      //create task
      var completeBy = new Date();
      completeBy.setDate(completeBy.getDate() + 7);
      var taskDesc = "Order No. " + this.member.orderNo + ' dt ' + this.member.orderDate + ' is released for your execution as per details provided' +
        '.  If you do not want to accept the task, you must decline it within 6 hours of receiving it.';
      var appTask= new ApplicationTask();
      appTask.taskTypeId= 2;    // **TODO - MAKE THIS DYNAMIC
      appTask.assignedToId= 2;    //**TODO - Make this dynamic */ */
      appTask.orderId = this.member.id;
      appTask.orderNo = this.member.orderNo;
      appTask.taskOwnerId = this.form.get('projectManagerId')!.value;
      appTask.completeBy = completeBy;
      appTask.postTaskAction = 3; //send email msg
      appTask.taskItems = [];
      appTask.taskStatus="Not started";
      appTask.taskDate=this.member.orderDate;
      appTask.taskDescription = taskDesc;

      this.taskService.createTaskFromAppTask(appTask).subscribe(() => {
        this.service.updateOrderWithDLFwdToHROn(this.member!.id, new Date()).subscribe(() => {
          //this.getMember(+this.routeId);
          console.log('updated Order for date forwarded');
        }, error => {
          console.log('failed to update order with dateforwarded to DL', error);
        })
        this.toastr.success('Task created in the name of the HR Supervisor');
      }, error => {
        this.toastr.error('failed to create task in the name of the HR Supervisor', error);
      })

    }

  
    forwardDLtoAgents() {
      
      if(this.member === null || this.member === undefined) return;

      //var selectedOrderItems= this.member.orderItems.filter(el => el.selected===true);
      //const selectedOrderItems = this.member.orderItems.map((checked, i) => checked ? checked[i].id : null).filter(v => v !== null);
      //var orderItemValues = this.orderItems.value;
      var selectedOrderItems = this.orderItems.value.filter((x:any) => x.selected===true && (x.reviewItemStatusId===1 || x.reviewItemStatusId===2 ));
      if (selectedOrderItems.length===0) {
        selectedOrderItems = this.orderItems.value.filter((x:any) => x.reviewItemStatusId===1 ||x.reviewItemStatusId===2 ); //select all that are accepted
        if(selectedOrderItems.length===0) {
          this.toastr.error('only items that are reviewed and accepted can be forwarded to agents');
          return;
        }
      }

      let agents = this.associates;
      const config = {
        class: 'modal-dialog-centered modal-lg windowlarge',
        //windowClass: 'large-Modal',
        initialState: {
          //order,
          agents
        }
      }

      this.bsModalRef = this.modalService.show(ChooseAgentsModalComponent, config);
      this.bsModalRef.content.updateSelectedOfficialIds.subscribe((values: any) => {
        const officialIdsToUpdate = {   //contains only selected official Ids
            agents: [...values.filter((el: any) => el.checked === true)]};
            for( var i = 0; i < agents.length; i++){ 
              if ( agents[i].checked) agents.splice(i,1);
            }
            //items -  charges, categoryname
        //dldates - customerofficialid, emailidforwardedto
          var dlforward = new dLForwardToAgent();
          
          dlforward.orderId!= this.member?.id;
          dlforward.orderNo!= this.member?.orderNo;
          dlforward.orderDate!=this.member?.orderDate;
          dlforward.customerId!= this.member?.customerId;
          dlforward.customerCity!= this.member?.cityOfWorking;
          dlforward.customerName!= this.member?.customerName;
          dlforward.projectManagerId!= this.member?.projectManagerId;

          var dlforwarditems: IDLForwardCategory[]=[];
          selectedOrderItems.forEach((i: any) => {
            var dlforwarditem = new dLForwardCategory();
            
            dlforwarditem.id=i.id;
            dlforwarditem.categoryId = i.categoryId;
            dlforwarditem.categoryName = this.member?.orderNo + '-' + i.srNo; + i.categoryName;
            dlforwarditem.charges = i.charges;
            dlforwarditem.orderItemId = i.id;
            dlforwarditem.orderId = i.orderId;

            agents.forEach(agt => {
              var dt = new dLForwardCategoryOfficial();
              dt.orderItemId = i.id;
              dt.customerOfficialId= agt.officialId;
              dt.agentName = agt.customerName;
              dt.dateTimeForwarded= new Date(); 
              dt.dateOnlyForwarded= new Date(); 
              dt.emailIdForwardedTo= agt.officialEmailId;
              dt.phoneNoForwardedTo= agt.mobileNo; 
              dt.whatsAppNoForwardedTo='';
              dt.loggedInEmployeeId!= this.user?.loggedInEmployeeId;

              dlforwarditem.dlForwardCategoryOfficials.push(dt)
            })
            dlforwarditems.push(dlforwarditem);
            
          })

          dlforward.dlForwardCategories = dlforwarditems;
          //check for unique constraints - OrderItemId, Dateforwarded.Date, OfficialId ** TODO **
          this.dlforwardService.forwardDLtoSelectedAgents(dlforward).subscribe((response) => {
            if(response ==='' || response===null) {
              this.toastr.success('Selected Order Categories forwarded to selected Associates');
            } else {
              this.toastr.error('Error forwarding the DLs to Associates' + response);
              console.log(response);
            }
          }, error => {
            this.toastr.error(error);
          })
      })
    }
  
    assignTasksToHRExecs() {
      var dt = new Date;
      var errors: string[]=[];

      if (this.isAddMode) {
        this.toastr.error('tasks cannot be assigned while in add mode.  Save the data first and then comeback to this form in edit mode to assign HR Executive tasks');
        return;
      }

      let f = this.form;
      
      var assignments:IOrderAssignmentDto[]=[]

      for (let i=0; i< f.value.orderItems.length;i++) {
        const element = f.value.orderItems.at(i);
        if (element.selected) {
            var assignment = new orderAssignmentDto;
            var hrexecid = element.hrExecId;
            if(hrexecid===null || hrexecid <= 0) {
              errors.push('category ' + element.categoryId + ' has not been assigned any HR Executive' );
              continue;
            }

            assignment.orderId=this.form.get('id')!.value;
            assignment.orderNo=this.form.get('orderNo')!.value;
            assignment.orderDate=this.form.get('orderDate')!.value;
            assignment.cityOfWorking=this.form.get('cityOfWorking')!.value;
            assignment.customerId=this.form.get('customerId')!.value;
            assignment.projectManagerId=this.form.get('projectManagerId')!.value;
            assignment.postTaskAction=3;
        
            assignment.orderItemId = element.id;
            assignment.hrExecId=element.hrExecId;
            assignment.categoryRef=assignment.orderNo + '-' + element.srNo;
            assignment.quantity = element.quantity;
            assignment.completeBy = element.completeBy?.getFullYear() < 2000 ? dt.getDate()+7 : element.completeBy;
            assignment.categoryId = element.categoryId;
            assignments.push(assignment);
        }
      }
      if(assignments.length===0) {
        this.toastr.warning('no valid categories found to assign. ' + (errors.length>0 ? errors.flat() : '' ));
        return;
      }

      this.confirmService.confirm("Confirm to proceed","Following error found: " + 
        errors.flat() + '. Do you want to exclude these tasks in the tasks?', 
        "Yes, Proceed", "No, Cancel").subscribe(result => {
          if(!result) return;
        }, error => {
          this.toastr.error(error);
          return;
        })

      // ** TODO ** use only HREXECID values that have changed
      if (assignments.length === undefined || assignments.length===0) {
        this.confirmService.confirm('you must select the DL Items that need to be assigned to HR Executives', 'order items not selected');
        return;
      };
      return this.taskService.createOrderAssignmentTasks(assignments).subscribe(resposne => {
        this.toastr.success('tasks created for the chosen order items');
      }, error => {
        this.toastr.error(error, 'failed to create tasks for the chosen order items');
      })

    }      
    
    customerChange(event: any) {
      if(event ===null) return;
      this.form.get('cityOfWorking')!.setValue(event.city);
      this.form.get('country')!.setValue(event.country);
      //this.selectedCustomerName = this.sharedService.getDropDownText(this.mySelect, this.customers[0].customerName);

    }
  
    /*
    projectManagerChange() {
      this.selectedProjManagerName = this.sharedService.getDropDownText(this.mySelect, this.employees[0].knownAs);
    }
    */
   
    /*
    minSelectedCheckboxes(min = 1) {
      const validator: ValidatorFn = (formArray: FormArray) => {
        const totalSelected = formArray.controls
          // get a list of checkbox values (boolean)
          .map(control => control.value)
          // total up the number of checked checkboxes
          .reduce((prev, next) => next ? prev + next : prev, 0);
    
        // if the total is not greater than the minimum, return the error message
        return totalSelected >= min ? null : { required: true };
      };
    
      return validator;
    }
*/
    openAssessmentModal(orderitemid: number) {
      if(orderitemid===undefined || orderitemid===0) return;
      this.router.navigateByUrl('/orders/itemassess/' + orderitemid);
    }

    showProspectives(srno: number) {
      this.router.navigateByUrl('/prospectives/prospectivelist/' + this.member?.orderNo + '-' + srno );
    }

    showProcess() {
      
    }

    reviewItems() {
      
    }

    formChanged() {
      console.log('form changed event');
    }

    qntyChanged(index: number) {
      if(+this.getMinCVs(index) > 0 ) return;

      var q = +this.getQnty(index);
      this.setMinCVs(index, q*3);
    }

}
