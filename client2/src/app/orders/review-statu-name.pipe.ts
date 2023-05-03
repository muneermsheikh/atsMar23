import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'reviewStatuName'
})
export class ReviewStatuNamePipe implements PipeTransform {

  statuses= [
    {id: 1, status: 'Accepted'}, {id: 2, status: 'Accepted with regrets'}, {id: 3, status: 'Regretted'},{id: 4, status: 'Not Reviewed'}
  ]
  transform(value: number): string {
    var dto = this.statuses.filter(x => x.id === value).map(x => x.status)[0];
    //console.log('in:', value, 'out:', dto);
    return dto;
  }

}
