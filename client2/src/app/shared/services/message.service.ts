import { Injectable } from '@angular/core';
import { ReplaySubject, of } from 'rxjs';
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

  baseUrl = environment.apiUrl;
  
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  mParams = new EmailMessageSpecParams();
  pagination: IPagination<IMessage[]> | undefined;  // = new PaginationMsg();
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
    setContainer(container: string) {
      this.container=container;
    }

    getMessages(useCache: boolean) { 
      
      if (useCache === false)  this.cache = new Map();
      
      if (this.cache.size > 0 && useCache === true) {
        if (this.cache.has(Object.values(this.mParams).join('-'))) {
          if (this.mParams.id! > 0)
          {
            this.pagination!.data = this.cache.get(Object.values(this.mParams.id!).join('-'));
          } else {
            this.pagination!.data = this.cache.get(Object.values(this.mParams).join('-'));
          }
          return of(this.pagination);
        }
      }

      let params = new HttpParams();

      if (this.mParams.search) {
        params = params.append('search', this.mParams.search);
      }

      params = params.append('sort', this.mParams.sort);
      params = params.append('pageIndex', this.mParams.pageIndex.toString());
      params = params.append('pageSize', this.mParams.pageSize.toString());
      params = params.append('container', this.mParams.container);

      this.http.get<IPagination<IMessage[]>>(this.baseUrl + 'messages/loggedinuser', {observe: 'response', params})
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.mParams).join('-'), response.body!.data);
            this.pagination = response.body!;
            return response.body;
          })
        )
      return;
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
    }
    
    getParams() {
      return this.mParams;
    }
}
