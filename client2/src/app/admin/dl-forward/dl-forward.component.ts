import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { IForwardedCategoryDto } from 'src/app/shared/dtos/admin/forwardedCategoryDto';
import { IForwardedDateDto } from 'src/app/shared/dtos/admin/forwardedOrderDto';
import { IDLForwardCategory } from 'src/app/shared/models/admin/dlForwardCategory';
import { IDLForwardToAgent } from 'src/app/shared/models/admin/dlforwardToAgent';
import { IUser } from 'src/app/shared/models/admin/user';

@Component({
  selector: 'app-dl-forward',
  templateUrl: './dl-forward.component.html',
  styleUrls: ['./dl-forward.component.css']
})
export class DlForwardComponent implements OnInit {

  forwards: IForwardedDateDto|undefined;
  routeId='';
  bolNavigationExtras: boolean =false;
  user?: IUser;
  returnUrl = 'orders';

  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router) {
    this.routeId = this.activatedRoute.snapshot.params['id'];
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              this.bolNavigationExtras=true;
              if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

              if( nav.extras.state.user) {
                this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
            }
    }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(response => {
      this.forwards = response.forwards;
      
      console.log('forwards',this.forwards, 'dlforward categories.component.ts');
    })
  }

  
  updateAgentsSelected() {

  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }
}
