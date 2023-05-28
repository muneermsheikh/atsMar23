import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'reviewItemStatusName'
})
export class ReviewItemStatusNamePipe implements PipeTransform {

  transform(value: number): string {

    if (value < 1 || value > 7) return 'undefined';

    var statuses= [
      {id: 1, status: 'Not Reviewed'}, {id: 2, status: 'Regret-Salary Not Feasible'}, 
      {id: 3, status: 'Regret-Facilities Not good'},{id: 4, status: 'Regret-Negative Background Info'},
      {id: 5, status: 'Regret-Visas Not Available'},{id: 6, status: 'Regret-Requiremt suspect'},
      {id: 7, status: 'Approved'}
    ]

    return statuses.filter(x => x.id==value).map(x => x.status)[0];
  }

}
