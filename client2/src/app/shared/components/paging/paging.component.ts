import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-paging',
  templateUrl: './paging.component.html',
  styleUrls: ['./paging.component.css']
})
export class PagingComponent implements OnInit {

  @Input() totalCount: number=0;
  @Input() pageSize: number=0;
  @Input() pageNumber: number=0;
  @Output() pageChanged = new EventEmitter<number>();
  
  constructor() { }

  ngOnInit(): void {
  }

  
  onPagerChanged(event: any){
    this.pageChanged.emit(event.page);
  }
  
}
