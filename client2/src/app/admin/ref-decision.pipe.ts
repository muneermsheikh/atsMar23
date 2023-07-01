import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'refDecision'
})
export class RefDecisionPipe implements PipeTransform {

  //values from EnumCVREfStatus, used in registering sel decisions
  statuses = [
    {id: 10, status: 'Selected'},
    {id: 11, status: 'Referred'},
    {id: 300, status: 'Rej-Not suitable'},
    {id: 400, status: 'Med Unfit'},
    {id: 3, status: 'Rej-Salary Exp High'},
    {id: 6, status: 'Rej-No Rel Exp'},
    {id: 12, status: 'Rej-Not qualified'},
    {id: 2, status: 'Rej-over age'},
    {id: 13, status: 'Not Interested'}
  ]
  transform(value: number) {
    
    if(value===0 || value===null || value===undefined ) {
      return 'undefined';
    }
    var dto = this.statuses.filter(x => x.id === value).map(x => x.status)[0];
    if(dto===null || dto===undefined) return 'undefined';
    return dto;
  }


}
