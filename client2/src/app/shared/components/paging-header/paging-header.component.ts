import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-paging-header',
  templateUrl: './paging-header.component.html',
  styleUrls: ['./paging-header.component.css']
})
export class PagingHeaderComponent implements OnInit {

  @Input() pageNumber: number=1;
  @Input() pageSize: number=10;
  @Input() totalCount: number=0;
  
  constructor() { }

  ngOnInit(): void {
  }

}
