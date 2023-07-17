import { Component, OnInit } from '@angular/core';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICOA, coa } from 'src/app/shared/models/masters/finance/coa';
import { CoaService } from '../coa.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-coa-edit',
  templateUrl: './coa-edit.component.html',
  styleUrls: ['./coa-edit.component.css']
})
export class CoaEditComponent implements OnInit {

 //@ViewChild('editForm') editForm: NgForm;

 routeId: string='';
 routeResumeId: string='';
 isAddMode: boolean=false;

 coa: ICOA = new coa(); //| undefined;

 user?: IUser;
 bolNavigationExtras:boolean=false;
 returnUrl: string='';
 
 constructor(
   private service: CoaService, 
   private bcService: BreadcrumbService, 
   private activatedRoute: ActivatedRoute, 
   private router: Router, 
   private toastr: ToastrService, 
 ) {
   this.routeId = this.activatedRoute.snapshot.params['id'];
   if(this.routeId==undefined) this.routeId='';
   
   //read navigationExtras
   let nav: Navigation|null = this.router.getCurrentNavigation();

   if (nav?.extras && nav.extras.state) {
       this.bolNavigationExtras=true;

       if(nav.extras.state.returnUrl) this.returnUrl=nav.extras.state.returnUrl as string;

       if(nav.extras.state.userobject) {
         this.user = nav.extras.state.userobject as IUser;
       }

       if( nav.extras.state.coatoedit) {
         this.coa = nav.extras.state.coatoedit as ICOA;
       }
   }
   if(this.routeId==='' && (this.coa===null || this.coa===undefined)) this.isAddMode=true;

   this.bcService.set('@editChartOfAccount',' ');

  }

 ngOnInit(): void {
   if(this.coa === undefined ) this.coa = new coa();
 }

 update() {

   if(this.isAddMode) {
       this.service.addNewCOA(this.coa).subscribe(response => {
         this.toastr.success('new Chart of Account added');
       }, error => {
         this.toastr.error('error in adding new Account', error);
       })
   } else {
       this.service.editCOA(this.coa).subscribe(response => {
         this.toastr.success('chart of account edited');
         this.returnToCaller();
       }, error => {
         this.toastr.error('failed to update the edits');
       })
   }
 }

 returnToCaller() {
   console.log('return to caller in coa-edit:', this.returnUrl);
   this.router.navigateByUrl(this.returnUrl || '' );
 }

 cancel() {
   this.returnToCaller();
 }

}
