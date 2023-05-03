import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-hr-index',
  templateUrl: './hr-index.component.html',
  styleUrls: ['./hr-index.component.css']
})
export class HrIndexComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    console.log('hr index');
  }

}
