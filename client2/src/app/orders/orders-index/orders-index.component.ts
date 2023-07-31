import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { catchError, switchMap, take, tap } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';
import { MastersService } from 'src/app/masters/masters.service';
import { orderParams } from 'src/app/shared/params/admin/orderParams';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { UserTaskService } from 'src/app/userTask/user-task.service';
import { OrderService } from '../order.service';
import { IContextMenuModel } from 'src/app/shared/models/admin/contextMenuModel';
import { IUser } from 'src/app/shared/models/admin/user';
import { IApplicationTask } from 'src/app/shared/models/admin/applicationTask';
import { ICustomerNameAndCity } from 'src/app/shared/models/admin/customernameandcity';
import { IProfession } from 'src/app/shared/models/masters/profession';
import { IOrderCity } from 'src/app/shared/models/admin/orderCity';
import { IOrderBriefDto } from 'src/app/shared/dtos/admin/orderBriefDto';
import { IMessage } from 'src/app/shared/models/admin/message';

@Component({
  selector: 'app-orders-index',
  templateUrl: './orders-index.component.html',
  styleUrls: ['./orders-index.component.css']
})
export class OrdersIndexComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  orders: IOrderBriefDto[]=[];
  oParams = new orderParams();
  totalCount: number=0;
  orderCities: IOrderCity[]=[];
  professions: IProfession[]=[];
  customers: ICustomerNameAndCity[]=[];

  task?: IApplicationTask;   //to show in task.edit component

  user?: IUser;
    
  //right click context menu
  title = 'context-menu';

  isDisplayContextMenu: boolean=false;
  rightClickMenuItems: Array<IContextMenuModel> = [];
  rightClickMenuPositionX: number=0;
  rightClickMenuPositionY: number=0;

  sortOptions = [
    {name:'By Order No Asc', value:'orderno'},
    {name:'By Order No Desc', value:'ordernodesc'},
    {name:'By City Asc', value:'city'},
    {name:'By City Desc', value:'citydesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Contract Reviewed Asc', value:'reviewed'},
    {name:'By Contract Rvwd Desc', value:'reveiweddesc'},
  ]

  orderStatus = [
    {name: 'Not Reviewed', value: 'NotReviewed'},
    {name: 'Reviewed and Approved', value: 'ReviewedAndApproved'},
    {name: 'Reviewed and declined', value: 'ReviewedAndDeclined'}
  ]

  constructor(private service: OrderService, 
    private mastersService: MastersService,
    private accountsService: AccountService,
    private taskService: UserTaskService,
    private router: Router,
    private toastr: ToastrService,
    private confirmService: ConfirmService) {
      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
     }

  ngOnInit(): void {
    this.service.setOParams(this.oParams);
    this.getOrders(true);
    this.getCities();
    this.getProfessions();
    
  }

  getOrders(useCache: boolean=false) {
    this.service.getOrdersBrief(useCache).subscribe({
      next: response => {
        this.orders = response.data;
        this.totalCount = response.count;
      },
      error: error => console.log(error)
    });
  }

  getCities() {
    this.service.getOrderCities().subscribe({
      next: response => this.orderCities = [{id: 999999, cityName: 'All'}, ...response],
      error: error => console.log(error)
    });
  }

  getProfessions() {
    this.mastersService.getCategoryList().subscribe({
      next: response => this.professions = [{id: 999999, name: 'All'}, ...response],
      error: error => console.log(error)
    });
  }

  getAgents() {
    this.mastersService.getAgents().subscribe({
      next: (response: any) => {
        this.professions = [{id: 99999999, name: 'All'}, ...response];
      },
      error: (error: any) => {
        console.log(error);
        this.toastr.error(error);
      }
    })
  }
  
  onSearch() {
    const params = this.service.getOParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setOParams(params);
    this.getOrders();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.oParams = new orderParams();
    this.service.setOParams(this.oParams);
    this.getOrders();
  }

  onSortSelected(event: any) {
    var sort = event?.target.value;
    this.oParams.pageNumber=1;
    this.oParams.sort = sort;
    this.getOrders();
  }
  
  onCitySelected(citySelected: string) {
    const prms = this.service.getOParams();
    prms.city = citySelected;
    prms.pageNumber=1;
    this.service.setOParams(prms);
    this.getOrders();
  }
  
  onProfSelected(profId: number) {
    const prms = this.service.getOParams();
    prms.categoryId = profId;
    prms.pageNumber=1;
    this.service.setOParams(prms);
    this.getOrders();

  }
  
  onCustomerSelected(agentId: number) {
    const prms = this.service.getOParams();
    prms.customerId = agentId;
    prms.pageNumber=1;
    this.service.setOParams(prms);
    this.getOrders();
  }

  onPageChanged(event: any){
    const params = this.service.getOParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.service.setOParams(params);
      this.getOrders(true);
    }
  }

  dlForward()
  {
    
  }

  displayContextMenu(event: any) {
    console.log('entered dispayconextmenu');

      this.isDisplayContextMenu = true;

      this.rightClickMenuItems = [
        {
          menuText: 'Refactor',
          menuEvent: 'Handle refactor',
        },
        {
          menuText: 'Format',
          menuEvent: 'Handle format',
        },
      ];

      this.rightClickMenuPositionX = event.clientX;
      this.rightClickMenuPositionY = event.clientY;
  }

  getRightClickMenuStyle() {
    return {
      position: 'fixed',
      left: `${this.rightClickMenuPositionX}px`,
      top: `${this.rightClickMenuPositionY}px`
    }
  }

  handleMenuItemClick(event: any) {
    switch (event.data) {
      case this.rightClickMenuItems[0].menuEvent:
           console.log('To handle refactor');
           break;
      case this.rightClickMenuItems[1].menuEvent:
          console.log('To handle formatting');
    }
  }
                    
  viewOrder(id: number) {
    this.navigateByRoute(id, 'orders/view', false);
  }

  deleteOrder (id: number){
    this.confirmService.confirm('confirm delete this Order', 'confirm delete order').pipe(
      switchMap(confirmed => this.service.deleteOrder(id).pipe(
        catchError(err => {
          console.log('Error in deleting the order', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted Order')),
        //tap(res=>console.log('delete voucher succeeded')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
        () => {
          console.log('delete succeeded');
          this.toastr.success('order deleted');
        },
        (err: any) => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      })
  }

  editOrder(id: number) {
    this.navigateByRoute(id, '/orders/edit', true);
  }

  remindSelDecisions(customerid: number) {
      this.service.remindClientForSelections(customerid).subscribe({
        next: (response: boolean) => {
          if(!response) {
            this.toastr.info('Failed to generate message for reminder to client');
        } else {
          this.toastr.success('Email Message for reminder to client generated.  It will be available in Messages Draft folder');
        }
      },
      error: error => this.toastr.error('Error in composing reminder message to customer')
    })

  }

  contractReviewOrder(id: number) {
    this.navigateByRoute(id, 'orders/review', true);
  }
  
  async dlForwardedToHRDept(event: any) {
    var orderid = event?.target.value;
    this.taskService.getTaskByOrderIdAndTaskType(orderid, 14).subscribe((response: any) => {
      this.task = response;
      let route = 'userTask/edittaskwithorderidandtasktype/' + orderid + '/' + 14;
      this.router.navigate(
          [route], 
          { state: 
            { 
              tasktoedit: this.task,
              user: this.user, 
              toedit: true, 
              returnUrl: '/orders' 
            } }
        );
  
    })
    
    //this.navigateByRoute(14, 'userTask/edittaskwithorderidandtasktype/' + orderid); //usertask/orderid/tasktypeid(14)
  }

  dlForwardToHRDept(event: any) {
    var id = event?.target.value;
    this.toastr.info(id);
  }

  dlForwardToAssociates(event: any) {

    var id = event;
     this.navigateByRoute(id, 'orders/forwards', true);
  }

  dlForwardedToAssociates(event: any) {

    var id = event;
     this.navigateByRoute(id, 'admin/forwarded', true);
  }

  acknowledgeToClient(event: any) {
    this.service.acknowledgeOrderToClient(event).subscribe({
      next: response => {
        if(response) {
          this.toastr.success('message of acknowledgement composed and saved in Messages Draft');
        } else {
          this.toastr.warning('failed to compose message of acknowledgement');
        }
      },
      error: err => this.toastr.error('Error in composing acknowldgement message to client', err)
    });
  }

  cvsReferred(event: any)
  {
    this.navigateByRoute(event, 'admin/cvreferred', false)
  }

  navigateByRoute(id: number, routeString: string, editable: boolean) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/orders' 
          } }
      );
  }
  
  @HostListener('document:click')
  documentClick(): void {
    this.isDisplayContextMenu = false;
  }

}
