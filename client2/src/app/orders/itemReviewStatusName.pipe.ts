import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'reviewStatuName'
})
export class itemReviewStatusNamePipe implements PipeTransform {

  statuses= [

    {id: 1, status: 'Not Reviewed'},{id: 2, status: 'Regretted-Salary'}, {id: 3, status: 'Regretted-Facilities'}, 
    {id: 4, status: 'Regretted-Background'},{id: 5, status: 'Regretted-Visas'}, {id: 6, status: 'Regretted-Suspect'},
    {id: 7, status: 'Approved'}
  ]
  transform(value: number): string {
    var dto = this.statuses.filter(x => x.id === value).map(x => x.status)[0];
    //console.log('in:', value, 'out:', dto);
    return dto;
  }

}
