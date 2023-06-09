import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ICustomer } from 'src/app/shared/models/admin/customer';
import { ICustomerCity } from 'src/app/shared/models/admin/customerCity';
import { IIndustryType } from 'src/app/shared/models/masters/profession';
import { CustomersService } from '../customers.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { ToastrService } from 'ngx-toastr';
import { ICustomerBriefDto } from 'src/app/shared/dtos/admin/customerBriefDto';
import { IPagination } from 'src/app/shared/models/pagination';
import { MastersService } from 'src/app/masters/masters.service';
import { paramsCustomer } from 'src/app/shared/params/admin/paramsCustomer';

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
      private toastrService: ToastrService
      ) {
   }

  ngOnInit(): void {
    this.custType = this.activatedRouter.snapshot.paramMap.get('custType')!;
    this.cParams.customerType=this.custType;
    
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

}
