import { Component, Input, OnInit } from '@angular/core';
import { ICustomer } from 'src/app/shared/models/admin/customer';

@Component({
  selector: 'app-customer-item',
  templateUrl: './customer-item.component.html',
  styleUrls: ['./customer-item.component.css']
})
export class CustomerItemComponent implements OnInit {

  @Input() cust: ICustomer | undefined;
  
  constructor() { }

  ngOnInit(): void {
  }

}
