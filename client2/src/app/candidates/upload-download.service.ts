import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FileToUpload } from '../shared/params/admin/fileToUpload';
import { Observable } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({
      'Content-Type': 'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class UploadDownloadService {

  apiUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  uploadFile(theFiles: FileToUpload[]) : Observable<any> {
    
    const uploadReq = new HttpRequest('POST', `api/FileUpload`, theFiles, {
      reportProgress: true,
    });

    return this.http.post<FileToUpload>(this.apiUrl + 'FileUpload/uploads', theFiles, httpOptions);
  }

  downloadFile(attachmentid: number) {
    return this.http.get(this.apiUrl + 'candidate/downloadfile/' + attachmentid);
  }

  
  downloadProspectiveFile(prospectiveid: number) {

    return this.http.get(this.apiUrl + 'FileUpload/downloadprospectivefile/' + prospectiveid);
  }

}
