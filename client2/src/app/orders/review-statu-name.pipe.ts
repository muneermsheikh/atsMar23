import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'reviewStatuName'
})
export class ReviewStatuNamePipe implements PipeTransform {

  statuses= [
    {id: 1, status: 'Not Reviewed'},{id: 4, status: 'Accepted'}, {id: 2, status: 'Accepted with regrets'}, 
    {id: 3, status: 'Regretted'}
  ]
  transform(value: number): string {
    if(value < 0 || value > 4) return 'undefined';
    var dto = this.statuses.filter(x => x.id === value).map(x => x.status)[0];
    //console.log('in:', value, 'out:', dto);
    return dto;
  }

}
