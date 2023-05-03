import { Pipe, PipeTransform } from '@angular/core';
import { OrderitemsService } from '../orders/orderitems.service';

@Pipe({
  name: 'categoryRefCode'
})
export class CategoryRefCodePipe implements PipeTransform {

  constructor(private service: OrderitemsService){}

  transform(value: number) {
    
    //console.log('entered pipe');
    
    if(value===0 || value===null || value===undefined ) {
      return 'undefined';
    }

    this.service.getOrderItem(value).subscribe(response => {
      return response.categoryRef;
    }, error => {
      return 'undefined';
    })
    
    return 'undefined';
  }

}
