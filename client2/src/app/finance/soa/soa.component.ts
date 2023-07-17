import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IStatementofAccountDto } from 'src/app/shared/dtos/finance/statementOfAccountDto';
import { IUser } from 'src/app/shared/models/admin/user';
import { soaParams } from 'src/app/shared/params/finance/soaParams';
import { CoaService } from '../coa.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-soa',
  templateUrl: './soa.component.html',
  styleUrls: ['./soa.component.css']
})
export class SoaComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;

  routeId: string='';
  user?: IUser;
  sParams = new soaParams();

  totaldr: number=0;
  totalcr: number=0;
  thisPeriodSuffix: string='';
  thisPeriodTotalDr: number=0;
  thisPeriodTotalCr: number=0;
  thisPeriodBalance: number=0;
  thisPeriodDiff: number=0;
  overallBal: number=0;
  overallTotalDr: number=0;
  overallTotalCr: number=0;
  overallBalSuffix: string='';

  soa: IStatementofAccountDto | undefined;
  
  bolNavigationExtras:boolean=false;
  returnUrl: string='';
  
  sortOptions = [
    {name:'By Account Name Asc', value:'name'},
    {name:'By Account Name Desc', value:'namedesc'},
    {name:'By Transaction Date Asc', value:'transdate'},
    {name:'By Transaction Date Desc', value:'trabsdatedesc'},
  ]

  constructor(
    private service: CoaService,
    private bcService: BreadcrumbService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) { 
      this.routeId = this.activatedRoute.snapshot.params['id'];
      if(this.routeId==undefined) this.routeId='';
      
      //read navigationExtras
      let nav: Navigation | null  = this.router.getCurrentNavigation();

      if (nav?.extras && nav.extras.state) {
          this.bolNavigationExtras=true;
          if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

          if(nav.extras.state.userobject) {
            this.user = nav.extras.state.userobject as IUser;
          }
      }
      this.bcService.set('@statementOfAccount',' ');
  }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => {
      this.soa = data.soa;
      
      if(this.soa !==undefined && this.soa!== null){

        this.thisPeriodTotalDr = this.soa.statementOfAccountItems.map(x => x.dr).reduce((a:number,b:number) => a+b,0);
        this.thisPeriodTotalCr = this.soa.statementOfAccountItems.map(x => x.cr).reduce((a:number,b:number) => a+b,0);
        this.thisPeriodDiff = this.thisPeriodTotalDr - this.thisPeriodTotalCr;
        this.thisPeriodSuffix = this.thisPeriodBalance > 0 ? ' DR' : ' CR';
        this.thisPeriodBalance = Math.abs(this.thisPeriodTotalDr-this.thisPeriodTotalCr);
        
        if(this.soa.opBalance > 0) {
          this.overallTotalDr = this.thisPeriodTotalDr + this.soa!.opBalance;
          this.overallTotalCr = this.thisPeriodTotalCr;
        } else {
          this.overallTotalCr = this.thisPeriodTotalCr + this.soa!.opBalance;
          this.overallTotalDr = this.thisPeriodTotalDr;
        }
        this.overallBal = this.overallTotalDr - this.overallTotalCr;

        this.overallBalSuffix = this.overallBal > 0 ? ' DR' : ' CR';
        this.overallBal=Math.abs(this.overallBal);
      } else {
        this.toastr.warning('No statement of account could be generated agaisnt the inputs');
        this.router.navigateByUrl(this.returnUrl);
      }
      
    })

  }

  
  returnToCaller() {
    this.router.navigateByUrl(this.returnUrl || '');
  }

  displayVoucherClicked(voucherno: number) {
    let route = '/finance/editfinancewithobject/' + voucherno;
    this.router.navigate([route], { state: { toedit: false, returnUrl: '/finance/voucherlist' } });
    //'statementofaccount/:id/:fromDate/:uptoDate
  }


}
