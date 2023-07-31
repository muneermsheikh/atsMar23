import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ICustomerCity } from 'src/app/shared/models/admin/customerCity';
import { IIndustryType } from 'src/app/shared/models/masters/profession';
import { CustomersService } from '../customers.service';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ICustomerBriefDto } from 'src/app/shared/dtos/admin/customerBriefDto';
import { IPagination } from 'src/app/shared/models/pagination';
import { MastersService } from 'src/app/masters/masters.service';
import { paramsCustomer } from 'src/app/shared/params/admin/paramsCustomer';
import { IUser } from 'src/app/shared/models/admin/user';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  customers: ICustomerBriefDto[]=[];
  customerCities: ICustomerCity[]=[];
  industryTypes: IIndustryType[]=[];
  cParams = new paramsCustomer();
  pagination?: IPagination<ICustomerBriefDto[]>;
  totalCount: number=0;
  custType: string='';
  user?: IUser;
  returnUrl ='';
  
  sortOptions = [
    {name:'By Name Asc', value:'name'},
    {name:'By Name Desc', value:'namedesc'},
    {name:'By City Asc', value:'city'},
    {name:'By City Desc', value:'citydesc'},
    {name:'By Industry Type Asc', value:'indtype'},
    {name:'By Industry Type Desc', value:'indtypedesc'}
  ]

  constructor(private service: CustomersService, 
      private activatedRouter: ActivatedRoute, 
      private mastersService: MastersService,
      private toastrService: ToastrService,
      private router: Router,
      private bcService: BreadcrumbService
      ) {
        let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

            if( nav.extras.state.user) {
              this.user = nav.extras.state.user as IUser;
              //this.hasEditRole = this.user.roles.includes('AdminManager');
              //this.hasHRRole =this.user.roles.includes('HRSupervisor');
            }
            
        }
        this.bcService.set('@deployentList',' ');
   }

  ngOnInit(): void {
    this.custType = this.activatedRouter.snapshot.paramMap.get('custType')!;
    this.cParams.customerType=this.custType;
    this.service.setCustParams(this.cParams);
    this.getCustomers();
    //this.getCities();
    //this.getIndustryTypes();

  }

  getCustomers() {
    this.service.getCustomers(false).subscribe(response => {
      this.pagination = response;
      this.cParams.pageNumber = response.pageIndex;
      this.cParams.pageSize = response.pageSize;
      this.totalCount = response.count;
      this.customers = response.data;
    }, error => {
      console.log(error);
    })
  }

  getCities() {
    this.service.getCustomerCities(this.custType).subscribe(response => {
      this.customerCities = [{cityName: 'All'}, ...response];
      //console.log('CUSTOMER CITIES', this.customerCities);
    })
    /*
    this.customerCities = [{cityName: 'All'}, ...this.customerCities];
    */
  }

  getIndustryTypes() {
    //this.industryTypes = [{id: 1, industryName:'real estate'}, {industryName: 'power generation'}, {industryName: 'power distribution'}];
    return this.mastersService.getIndustries().subscribe(response => {
      this.industryTypes = response;
    }, error => {
      this.toastrService.error('failed to get industry types from api');
    })
  }

  onCitySelected(event: any) {
    var citySelected = event?.target.value;
    this.cParams.customerCityName = citySelected;
    this.cParams.pageNumber = 1;
    this.getCustomers();
  }

  onSortSelected(event: any) {
    var sort = event?.target.value;
    this.cParams.sort = sort;
    this.getCustomers();
  }

  onIndTypeSelected(event: any) {
    var typ = event.target.value;
    this.cParams.customerIndustryId = typ;
    this.getCustomers();
  }


  onSearch() {
    console.log('onsearch', this.searchTerm?.nativeElement.value);
    this.cParams.search = this.searchTerm?.nativeElement.value;
    this.cParams.pageNumber = 1;
    this.getCustomers();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = null;
    this.cParams = new paramsCustomer();
    this.getCustomers();
  }

  onPageChanged(event: any){
    //if (this.customerParams.pageNumber !== event) {
        this.cParams.pageNumber = event;
        this.getCustomers();
    //}
  }

  editCustomer(customerid: number) {
    this.navigateByRoute(customerid, '/customers/edit', true)
  }

  navigateByRoute(id: number, routeString: string, editable: boolean ) {
    let route = routeString;
    if(id!==0) route = route + '/' + id;
    
    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/customers/customerlist/' + this.custType,
          } }
      );
  }

  addCustomer() {
    this.navigateByRoute(0, '/customers/add/' + this.custType, true)    
  }
}
