import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ICustomer } from 'src/app/shared/models/admin/customer';
import { IUser } from 'src/app/shared/models/admin/user';
import { IIndustryType, IProfession } from 'src/app/shared/models/masters/profession';
import { CustomersService } from '../../customers.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ICustomerOfficial } from 'src/app/shared/models/admin/customerOfficial';
import { ICustomerIndustry } from 'src/app/shared/models/admin/customerIndustry';
import { IAgencySpecialty } from 'src/app/shared/models/admin/agencySpecialty';
import { MastersService } from 'src/app/masters/masters.service';

@Component({
  selector: 'app-client-edit',
  templateUrl: './client-edit.component.html',
  styleUrls: ['./client-edit.component.css']
})
export class ClientEditComponent implements OnInit {

  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent | undefined;
  activeTab: TabDirective | undefined;
  @ViewChild('editForm') editForm: NgForm | undefined;
  routeId: string;
  member: ICustomer | undefined;
  user: IUser | undefined;
  
  form: FormGroup=new FormGroup({});
  selectedCategoryIds: number[]=[];
  categories: IProfession[]=[];
  industries: IIndustryType[]=[];

  isAddMode: boolean=false;
  loading = false;
  submitted = false;

  lastFormValue: any | undefined;

  returnUrl: string = '/customers';

  errors: string[]=[];

  constructor(private service: CustomersService, private bcService: BreadcrumbService, 
      private activatedRoute: ActivatedRoute, private router: Router, private sharedService: MastersService
      //, private accountService: AccountService
      , private toastr: ToastrService, private fb: FormBuilder) {
          this.bcService.set('@customerDetail',' ');
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
          this.bcService.set('@editCustomer',' ');

   }

  ngOnInit(): void {
      this.createForm();
      this.isAddMode = !this.routeId;
      this.getMember(+this.routeId);
  }

  
  getMember(id: number) {
    this.service.getCustomer(id).subscribe( 
      {
        next: response => {
          this.member = response;
        this.patchCustomer(this.member);

        //console.log('customer:', this.member);
        if(this.member?.customerType ==='customer') this.getIndustries();
        if(this.member?.customerType !=='customer') this.getCategories();
        },
        error: err => console.log('error:', err)
      }
    )
    

  }


  createForm() {
    this.form = this.fb.group({
      id: [null],
      customerType: ['', [Validators.required, 
        Validators.maxLength(10), Validators.minLength(5)]],
      customerName: ['', [Validators.required,
        Validators.maxLength(50), Validators.minLength(5)]],
      knownAs: ['', [Validators.required,
        Validators.maxLength(20), Validators.minLength(4)]],
      add: '',
      add2: '', 
      city: ['', [Validators.required,
        Validators.maxLength(25), Validators.minLength(5)]],
      pin: '', 
      district: '',
      state: '', country: '',
      email: [null, 
        [Validators.required, Validators.email, Validators
        .pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]],
      website: '',
      phone: '',
      phone2: '', 
      logoUrl: '',
      customerStatus: 0,
      createdOn: '',
      introduction: '',
      customerIndustries: this.fb.array([]),
      customerOfficials: this.fb.array([]),
      agencySpecialties: this.fb.array([])
      
    } 
    );
  }

    patchCustomer(cv: ICustomer) {
      if(cv!==undefined && cv!==null) {
          this.form.patchValue( {
            id: cv.id, customerType: cv.customerType, customerName: cv.customerName, 
            knownAs: cv.knownAs, add: cv.add, add2: cv.add2, city: cv.city,
            pin: cv.pin, district: cv.district, state: cv.state, country: cv.country,
            email: cv.email, website: cv.website, phone: cv.phone, phone2: cv.phone2,
            logoUrl: cv.logoUrl, customerStatus: cv.customerStatus, createdOn: cv.createdOn,
            introduction: cv.introduction, 
          });

          if (cv.customerOfficials != null) {
            this.form.setControl('customerOfficials', this.setExistingCustomerOfficials(cv.customerOfficials));
          }

          if (cv.customerIndustries != null) {
            this.form.setControl('customerIndustries', this.setExistingCustomerIndustries(cv.customerIndustries));
          }
          
          if (cv.agencySpecialties != null) {
            this.form.setControl('agencySpecialties', this.setExistingAgencySpecialties(cv.agencySpecialties));
        }
      }
    }

    
    setExistingCustomerOfficials(off: ICustomerOfficial[]): FormArray {
        const formArray = new FormArray([]);
        off.forEach(ph => {
          formArray.push(this.fb.group({
            id: ph.id,
            customerId: ph.customerId,
            logInCredential: ph.logInCredential,
            appUserId: ph.appUserId,
            gender: ph.gender,
            title: ph.title,
            officialName: ph.officialName,
            designation: ph.designation,
            divn: ph.divn,
            phoneNo: ph.phoneNo,
            mobile: ph.mobile,
            email: ph.email,
            imageUrl: ph.imageUrl,
            isValid: ph.isValid
          }))
        });
        return formArray;
    }

    setExistingCustomerIndustries(inds: ICustomerIndustry[]): FormArray {
        const formArray = new FormArray([]);
        inds.forEach(q => {
          formArray.push(this.fb.group({
            id: q.id,
            customerId: q.customerId,
            industryId: q.industryId,
            name: q.name
          }))
        });
        return formArray;
    }

    setExistingAgencySpecialties(specs: IAgencySpecialty[]): FormArray {
      const formArray = new FormArray([]);
      specs.forEach(p => {
        formArray.push(this.fb.group({
          id: p.id,
          customerId: p.customerId,
          professionId: p.professionId,
          name: p.name,
        }))
      });
      return formArray;
    }  

    get customerOfficials() : FormArray {
        return this.form.get("customerOfficials") as FormArray
      }
    
    newCustomerOfficial(): FormGroup {
        return this.fb.group({
          id: 0, customerId: 0, logInCredential: true, 
          appUserId: '', 
          gender: ['',[ 
              Validators.required, Validators.maxLength(1), 
              Validators.minLength(1)]], 
          title: '', 
          officialName: ['', [Validators.required, Validators.maxLength(25)]], 
          designation: '', 
          divn: ['', [Validators.required, Validators.maxLength(10)]], 
          phoneNo: '',
          mobile: ['', [Validators.required, Validators.minLength(10)]],
          email:  [null, [Validators.required, 
            Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]],
          imageUrl: '', isValid: true
        })
      }

      addCustomerOfficial() {
        this.customerOfficials.push(this.newCustomerOfficial());
      }
      removeCustomerOfficial(i:number) {
        this.customerOfficials.removeAt(i);
        this.customerOfficials.markAsDirty();
        this.customerOfficials.markAsTouched();
      }

      get customerIndustries() : FormArray {
        return this.form.get("customerIndustries") as FormArray
      }

      newCustomerIndustry(): FormGroup {
        var customerid=this.member?.id ?? 0;
        return this.fb.group({
          id: 0, customerId: customerid, industryId: 0, name: ''
        })
      }

      addCustomerIndustry() {
        this.customerIndustries.push(this.newCustomerIndustry());
      }

      removeCustomerIndustry(i:number) {
        this.customerIndustries.removeAt(i);
        this.customerIndustries.markAsDirty();
        this.customerIndustries.markAsTouched();
      }

// userProfessions
      get agencySpecialties() : FormArray {
        return this.form.get("agencySpecialties") as FormArray
      }
    
      newAgencySpecialty(): FormGroup {
        return this.fb.group({
          id: 0, 
          customerId: [0, Validators.required], 
          industryId: 0, 
          name: ['', Validators.required]
        })
      }
    
      addAgencySpecialty() {
        this.agencySpecialties.push(this.newAgencySpecialty());
      }  
    
      removeAgencySpecialty(i:number) {
        this.agencySpecialties.removeAt(i);
        this.agencySpecialties.markAsDirty();
        this.agencySpecialties.markAsTouched();
      }

  // various gets from APis

      getCategories() {
        console.log('getting categories');
        this.sharedService.getCategoryList().subscribe((response: any) => {
          this.categories = response;
        }, (error: any) => {
          console.log(error);
        })
      }

      getIndustries() {

        this.sharedService.getIndustries().subscribe(response => {
          this.industries = response;
          console.log('industries:', this.industries);
        }, error => {
          this.toastr.error('failed to get the industries', error);
        })
      }

      onSubmit() {
        if(this.lastFormValue === this.form.value) {
          console.log('same form value returned');
          return;
        }

        if (+this.routeId ===0) {
            this.CreateCustomer();
        } else {
            this.UpdateCustomer();
        }
        this.lastFormValue = this.form.value;
      }

      private CreateCustomer() {
        this.service.register(this.form.value).subscribe(() => {
        }, (error:any) => {
          console.log(error);
          this.errors = error.errors;
        })
      }

      private UpdateCustomer() {
        /*
        let formData = new FormData();
        formData=this.form.value;
        this.selectedFiles.forEach((f) => {
          console.log(f);
          if (f !== null) {
            formData.append('userFormFiles', f);
        }})
        */


        this.service.updateCustomer(this.form.value).subscribe(() => {
          this.toastr.success('customer updated');
          this.close();

        }, (error:any) => {
          console.log(error);
        })
      }

      showReview() {
        this.router.navigateByUrl('/customers/review/' + this.routeId);
      }
    
      selectTab(tabId: number) {
        this.memberTabs!.tabs[tabId].active = true;
      }


      onTabActivated(data: TabDirective) {
        this.activeTab = data;
      }

      close() {
        this.router.navigateByUrl(this.returnUrl);
      }

     /* navigateByRoute(routeString: string, obj: any,  editable: boolean) {
        let route =  routeString;
        this.router.navigate(
            [route], 
            { state: 
              { 
                user: this.user, 
                object: obj,
                toedit: editable, 
                returnUrl: '/customers'
              } }
          );
      }
      */
}
