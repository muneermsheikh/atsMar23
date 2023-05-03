import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-prospective-list',
  templateUrl: './prospective-list.component.html',
  styleUrls: ['./prospective-list.component.css']
})
export class ProspectiveListComponent implements OnInit {

  form: NgForm|undefined;

  constructor() { }

  ngOnInit(): void {
  }

}
