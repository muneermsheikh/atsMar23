import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';
import { ICVReferredDto } from 'src/app/shared/dtos/admin/cvReferredDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { CVRefParams } from 'src/app/shared/params/admin/cvRefParams';
import { BreadcrumbService } from 'xng-breadcrumb';
import { CvrefService } from '../cvref.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cvreferred',
  templateUrl: './cvreferred.component.html',
  styleUrls: ['./cvreferred.component.css']
})
export class CvreferredComponent implements OnInit {
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  cParams = new CVRefParams();
  totalCount = 0;

  user?: IUser;
  returnUrl = '/admin';
  referred: ICVReferredDto[]=[];

  constructor(private accountsService: AccountService
      , private router: Router
      , private bcService: BreadcrumbService
      , private activatedRoute: ActivatedRoute
      , private service: CvrefService
      , private toastr: ToastrService) {
      /*this.routeId = this.activatedRoute.snapshot.params['id'];
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
      */

      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

      //navigationExtras
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

          if( nav.extras.state.user) {
              this.user = nav.extras.state.user as IUser;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
          this.bcService.set('@cvs Referred',' ');
   }

   
  sortOptions = [
    {name:'By Order No Asc', value:'orderno'},
    {name:'By Order No Desc', value:'ordernodesc'},
    {name:'By Order Cateogory Asc', value:'category'},
    {name:'By Order Category Desc', value:'categordesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Date Referred Asc', value:'datereferred'},
    {name:'By Date Referred Desc', value:'datereferreddesc'},
  ]

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.referred = data.referredcvs.data;
      this.totalCount = data.referredcvs.count;
      
    })
  }

  getReferred(useCache=false) {
    this.service.referredCVs(useCache).subscribe({
      next: response => {
        this.referred = response.data;
        this.totalCount = response.count;
        if(this.referred===null) this.toastr.info('failed to retrieve data');
      },
      error: error => console.log(error)
    });
  }
  

  onPageChanged(event: any){
    const params = this.service.getCVRefParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setCVRefParams(params);
      this.getReferred(true);
    }
  }

  
  onSearch() {
    const params = this.service.getCVRefParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setCVRefParams(params);
    this.getReferred();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cParams = new CVRefParams();
    this.service.setCVRefParams(this.cParams);
    this.getReferred();
  }

  onSortSelected(event: any) {
    var sort = event?.target.value;
    this.cParams.pageNumber=1;
    this.cParams.sort = sort;
    this.getReferred();
  }
  
  remindClient(event: any) {

  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }

}
