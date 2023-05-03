import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../shared/models/admin/user';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';
import { ITaskDashboardDto } from '../shared/dtos/admin/taskDashboardDto';


@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser | null>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  //private dashboardSource = new ReplaySubject<ITaskDashboardDto[]>();
//dashboardTask$ = this.dashboardSource.asObservable();
  
  constructor(private http: HttpClient, private router: Router, private toastr: ToastrService
    //, private presence: PresenceService
    ) { }

    login(model: any) {
      return this.http.post<IUser>(this.baseUrl + 'account/login', model).pipe(map((response: IUser) => {
        const user = response;
          if(user) {
            this.setCurrentUser(user); 
          }
        }
      ))

    }
  
    register(model: any) {
      return this.http.post<IUser>(this.baseUrl + 'account/register', model).pipe(
        map((user: IUser) => {
          if(user) {
            this.setCurrentUser(user);
            this.currentUserSource.next(user);
          }
        })
      )
    }
  
    setCurrentUser(user: IUser) {
      //console.log('in setcurrentuser, user is: ', user);
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
      Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
      //console.log('user roles in setCurrentUser : ',user.roles);
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('token', user.token);
      this.currentUserSource.next(user);

      //this.baseUrl + 'task/pendingtasksofloggedinuser/1/10';
    }

    setDashboardTasks(tasks:ITaskDashboardDto[])
    {
      //this.dashboardSource.next(tasks);
    }

    logout() {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      this.currentUserSource.next(undefined);
      this.router.navigateByUrl('/');
      //this.presence.stopHubConnection();
    }

    getDecodedToken(token: string) {
      return JSON.parse(atob(token.split('.')[1]));
    }

    checkEmailExists(email: string) {
      return this.http.get<boolean>(this.baseUrl + 'account/emailexists?email=' + email);
    }

    checkPPExists(ppnumber: string) {
      return this.http.get(this.baseUrl + 'account/ppexists?ppnumber=' + ppnumber);
    }

    checkAadharExists(aadharno: string) {
      return this.http.get(this.baseUrl + 'account/aadharnoexsts?aadahrno=' + aadharno);
    }

    getCandidate(id: number) {
      return this.http.get(this.baseUrl + 'candidate/byid/' + id);
    }

    loadCurrentUser(token: string | null) {
      if (token == null) {
        this.currentUserSource.next(null);
        return of(null);
      }

      let headers = new HttpHeaders();
      headers = headers.set('Authorization', `Bearer ${token}`);

      return this.http.get<IUser>(this.baseUrl + 'account', {headers}).pipe(
        map(user => {
          if (user) {
            localStorage.setItem('token', user.token);
            this.currentUserSource.next(user);
            return user;
          } else {
            return null;
          }
        })
      )
    }
}
