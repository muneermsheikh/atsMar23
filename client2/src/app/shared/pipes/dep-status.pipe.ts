import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'depStatus'
})
export class DepStatusPipe implements PipeTransform {

  //values from EnumDeloyStatus.  also depstatus table
  statuses= [
    {sequence: 0, status: 'None'}, 
    {sequence: 100, status: 'Selected'}, 
    {sequence: 200, status: 'Document Certificate Initiated'},
    {sequence: 1400, status: 'Docs couriered to candidate'},
    {sequence: 1300, status: 'Travel Tkt booked'},
    {sequence: 1200, status: 'Emig Denied'},
    {sequence: 1100, status: 'Emig Granted'},
    {sequence: 1009, status: 'Emig Documents Lodged'},
    {sequence: 700, status: 'Visa Endorsed'}, 
    {sequence: 600, status: 'Visa Docs submitted'}, 
    {sequence: 500, status: 'Med Unfit'},
    {sequence: 400, status: 'Med Fit'}, 
    {sequence: 800, status: 'Visa denied'},
    {sequence: 5000, status: 'Concluded'},
    {sequence: 18, status: 'Visa Docs prepared'},
    {sequence: 2000, status: 'Offer Letter Accepted'},
    {sequence: 300, status: 'Ref For Med Tests'}
  ]

  transform(value: number): string {
    var dto = this.statuses.filter(x => x.sequence === value).map(x => x.status)[0];
    if(dto===null || dto===undefined) return 'undefined';
    return dto;
  }
}
