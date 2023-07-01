import { Component, OnInit } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { IUser } from 'src/app/shared/models/admin/user';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-deploy-index',
  templateUrl: './deploy-index.component.html',
  styleUrls: ['./deploy-index.component.css']
})
export class DeployIndexComponent implements OnInit {

  user?: IUser;
  returnUrl = '';

  constructor(
    private router: Router
    , private bcService: BreadcrumbService
  ) { 
    let nav: Navigation|null = this.router.getCurrentNavigation() ;

    if (nav?.extras && nav.extras.state) {
        if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

        if( nav.extras.state.user) {
          this.user = nav.extras.state.user as IUser;
          //this.hasEditRole = this.user.roles.includes('AdminManager');
          //this.hasHRRole =this.user.roles.includes('HRSupervisor');
        }
        if(nav.extras.state.user) this.user=nav.extras.state.user as IUser;
    }
    this.bcService.set('@deploymentIndex',' ');
  }

  ngOnInit(): void {
    
  }

  return() {
    this.router.navigateByUrl('');
  }

  showDeploymentList() {
    this.navigateByRoute(0, '/processing/list', false);
  }

  navigateByRoute(id: number, routeString: string, editable: boolean) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/processing/list' 
          } }
      );
  }
}
