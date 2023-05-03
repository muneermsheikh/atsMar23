import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IApiReturnDto } from '../dtos/admin/apiReturnDto';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  public updateWithFiles(formData: FormData) {
    return this.http.put(this.apiUrl + 'candidate/updatecandidate', formData, {
      reportProgress: true,
      observe: 'events',
    });
  }

  public registerNewCandidate(formData: FormData) {
      return this.http.post<IApiReturnDto>(this.apiUrl +  'account/RegisterNewCandidate', formData, {
        reportProgress: true,
        observe: 'events',
    });
  }

  public download(attachmentid: number) {
    return this.http.get(this.apiUrl + 'upload/download/' + attachmentid, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }

}
