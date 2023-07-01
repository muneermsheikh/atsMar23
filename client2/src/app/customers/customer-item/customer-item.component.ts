import { Component, Input, OnInit } from '@angular/core';
import { ICustomerBriefDto } from 'src/app/shared/dtos/admin/customerBriefDto';
import { ICustomer } from 'src/app/shared/models/admin/customer';

@Component({
  selector: 'app-customer-item',
  templateUrl: './customer-item.component.html',
  styleUrls: ['./customer-item.component.css']
})
export class CustomerItemComponent implements OnInit {

  @Input() cust: ICustomerBriefDto | undefined;
  
  constructor() { }

  ngOnInit(): void {
  }

}
