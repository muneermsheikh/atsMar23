import { Pipe, PipeTransform } from '@angular/core';
import { OrderitemsService } from '../orders/orderitems.service';

@Pipe({
  name: 'categoryRefCode'
})
export class CategoryRefCodePipe implements PipeTransform {

  constructor(private service: OrderitemsService){}

  transform(value: number) {
    
    if(value===0 || value===null || value===undefined ) {
      return 'undefined';
    }

    this.service.getOrderItemRefCode(value).subscribe(response => {
      console.log('categorrefcodeipe response', response);
      return response;
    }, error => {
      console.log('error in value-', value, error);
      return 'undefined';
    })
    
    return 'undefined';
  }

}
