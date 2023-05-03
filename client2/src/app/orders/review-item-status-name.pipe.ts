import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'reviewItemStatusName'
})
export class ReviewItemStatusNamePipe implements PipeTransform {

  transform(value: number): string {

    var statuses= [
      {id: 1, status: 'Accepted'}, {id: 2, status: 'Requirement Suspect'}, 
      {id: 3, status: 'Visa Not Avbl'},{id: 4, status: 'Negative Background Info'},
      {id: 5, status: 'Facilities Insufficient'},{id: 6, status: 'Salary Not Feasible'},
      {id: 7, status: 'Not Reviewed'}
    ]
    return statuses.filter(x => x.id==value).map(x => x.status)[0];
  }

}
