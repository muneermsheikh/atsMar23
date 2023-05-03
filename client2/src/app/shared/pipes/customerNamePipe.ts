import { Pipe, PipeTransform } from "@angular/core";

import { CustomersService } from "src/app/customers/customers.service";

@Pipe({
    name: 'customerName'
})

export class CustomerNamePipe implements PipeTransform {
 
    cname='';
    constructor(private custService: CustomersService){}
    
     transform(customerId: number) {
        if(customerId && !isNaN(customerId)) {
            var cname = this.custService.getCustomerNameFromId(customerId).subscribe(response => this.cname = response!);
            //console.log("customeId:", customerId, cname);
            return cname;
        }
        return;
    }
    
}