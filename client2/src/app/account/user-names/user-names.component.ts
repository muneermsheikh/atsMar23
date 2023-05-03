import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-user-names',
  templateUrl: './user-names.component.html',
  styleUrls: ['./user-names.component.css']
})
export class UserNamesComponent implements OnInit {

  @Input() registerForm?: FormGroup
  
  maxDate: Date = new Date();

  constructor() { }

  ngOnInit(): void {
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

 

}
