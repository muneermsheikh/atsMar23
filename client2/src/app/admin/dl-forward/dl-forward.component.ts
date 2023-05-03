import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IDLForwardCategory } from 'src/app/shared/models/admin/dlForwardCategory';
import { IDLForwardToAgent } from 'src/app/shared/models/admin/dlforwardToAgent';

@Component({
  selector: 'app-dl-forward',
  templateUrl: './dl-forward.component.html',
  styleUrls: ['./dl-forward.component.css']
})
export class DlForwardComponent implements OnInit {

  forwards: IDLForwardToAgent | undefined;
  forwardedCategories: IDLForwardCategory[] | undefined;

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(response => {
      this.forwards = response.forwards;
      this.forwardedCategories = this.forwards?.dlForwardCategories;
      
      console.log('dlforward.component.ts', this.forwards);
    })
  }

  
  updateAgentsSelected() {

  }

}
