import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { IHelp } from "../shared/models/admin/help";
import { HelpService } from "../shared/services/help.service";



@Injectable({
	providedIn: 'root'
  })
  export class HelpResolver implements Resolve<IHelp> {
  
	constructor(private service: HelpService) {}
  
	resolve(route: ActivatedRouteSnapshot): Observable<IHelp> {
		
		var topic = route.paramMap.get('topic');
		
	   	return this.service.getHelp(topic);
	}
  
  }