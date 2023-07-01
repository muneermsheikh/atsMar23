import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customerStatus'
})
export class customerStatusPipe implements PipeTransform {

  statuses= [
    {id: 100, status: 'Active'},{id: 200, status: 'Closed'}, {id: 300, status: 'Blacklisted'}
  ]
  transform(value: number): string {
    var dto = this.statuses.filter(x => x.id === value).map(x => x.status)[0];
    if(dto===null || dto===undefined) return 'undefined';
    return dto;
  }

}
