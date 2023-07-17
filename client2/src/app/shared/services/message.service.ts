import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUser } from '../models/admin/user';
import { EmailMessageSpecParams } from '../params/admin/emailMessageSpecParams';
import { IPagination } from '../models/pagination';
import { IMessage } from '../models/admin/message';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  [x: string]: any;

  baseUrl = environment.apiUrl;
  
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  mParams = new EmailMessageSpecParams();
  pagination?: IPagination<IMessage[]>  // = new PaginationMsg();
  cache = new Map();

  container: string='';

  constructor(private http: HttpClient) { }

  /*
  getMessages(pageNumber, pageSize, container) {
    //let params = getPaginationHeaders(pageNumber, pageSize);
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }
*/
    setContainer() {
      this.container=this.mParams.container;  // container;
    }

    getMessages(useCache: boolean): Observable<IPagination<IMessage[]>> { 
      
      if (useCache === false)  this.cache = new Map();
      
      if (this.cache.size > 0 && useCache === true) {
        if (this.cache.has(Object.values(this.mParams).join('-'))) {
            this.pagination = this.cache.get(Object.values(this.mParams).join('-'));
            if(this.pagination)return of(this.pagination);
        }
      }

      let params = new HttpParams();

      if (this.mParams.search) params = params.append('search', this.mParams.search);
      
      params = params.append('sort', this.mParams.sort);
      params = params.append('pageIndex', this.mParams.pageIndex.toString());
      params = params.append('pageSize', this.mParams.pageSize.toString());
      params = params.append('container', this.mParams.container);

      console.log('messages from api', params);

      return this.http.get<IPagination<IMessage[]>>(this.baseUrl + 'messages/loggedinuser', {params}).pipe(
        map(response => {
          this.cache.set(Object.values(this.mParams).join('-'), response);
          this.pagination = response;
          return response;
        })
      )
      /*
      this.http.get<IPagination<IMessage[]>>(this.baseUrl + 'messages/loggedinuser', {params})
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.mParams).join('-'), response);
            this.pagination = response;
            return response;
          })
        )
        console.log('returning null');
        return of();
      */
    }

    getMessageThread(username: string, pageno: number, pagesize: number) {
      return this.http.get<IMessage[]>(this.baseUrl + 'messages/msgthreadforuser/' + username) + '/' + pageno + '/' + pagesize;
    }

    sendMessage(msg: IMessage) {
      return this.http.post<IMessage>(this.baseUrl + 'messages', msg);

    }

    saveMessage(msg: IMessage) {
      return this.http.put<IMessage>(this.baseUrl + 'messages/savemessage', {msg});
    }
    
    deleteMessage(id: number) {
      return this.http.delete(this.baseUrl + 'messages/' + id);
    }

    setParams(params: EmailMessageSpecParams) {
      this.mParams = params;
      this.container = params.container;
    }
    
    getParams() {
      return this.mParams;
    }
}
