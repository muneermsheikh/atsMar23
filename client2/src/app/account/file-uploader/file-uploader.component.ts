import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { IUser } from 'src/app/shared/models/admin/user';
import { ICandidate } from 'src/app/shared/models/hr/candidate';
import { environment } from 'src/environments/environment';
import { AccountService } from '../account.service';
import { CandidateService } from 'src/app/candidates/candidate.service';
import { take } from 'rxjs/operators';
import { IUserAttachment } from 'src/app/shared/models/hr/userAttachment';

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.css']
})
export class FileUploaderComponent implements OnInit {

  @Input() member: ICandidate | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: IUser | undefined;

  constructor(private accountService: AccountService, private candidateService: CandidateService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user
      }
    })
   }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  setPhoto(photo: IUserAttachment) {
    this.candidateService.setPhoto(photo).subscribe({
      next: () => {
        if (this.member) {
          this.member.photoUrl = photo.url;
          //this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url;
          /*this.member.userAttachments.forEach(p => {
            if (p.attachmentType==='photo') p.isMain = false;
            if (p.id === photo.id) p.isMain = true;
          })
          */
        }
      }
    })
  }

  deleteAttachment(attachmentId: number) {
    this.candidateService.deleteAttachment(attachmentId).subscribe({
      next: _ => {
        if (this.member) {
          this.member.userAttachments = this.member.userAttachments.filter(x => x.id !== attachmentId);
        }
      }
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image', 'pdf', 'doc'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        /*const photo = JSON.parse(response);
        this.member?.photos.push(photo);
        if (photo.isMain && this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
        */
       const attachment = JSON.parse(response);
       this.member?.userAttachments.push(attachment);
       if(attachment.attachmentType==='photo' && this.member !== undefined) 
          this.member.photoUrl=attachment.url;
       
        
      }
    }
  }

}
