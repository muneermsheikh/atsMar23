import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';


// https://www.concretepage.com/angular/angular-file-upload

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  baseUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    uploadWithProgress(formData: FormData): Observable<any> {
        return this.http.post(this.baseUrl + 'fileupload', formData, { observe: 'events',  reportProgress: true })
            .pipe(
                catchError(err => this.handleError(err))
            );
    }
    private handleError(error: any) {
        return throwError(error);
    }
}
