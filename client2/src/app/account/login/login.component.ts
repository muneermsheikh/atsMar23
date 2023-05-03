import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IUser } from 'src/app/shared/models/admin/user';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup = new FormGroup({
    email: new FormControl('munir.sheikh@live.com', [Validators.required, Validators.email]),
    password: new FormControl('Pa$$w0rd', Validators.required),
  });
  
  returnUrl: string | undefined;
  user: IUser | null | undefined;

  constructor(private accountService: AccountService, 
      private router: Router, private activatedRoute: ActivatedRoute) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/userTask';
    //console.log('returnUrl:', this.returnUrl);
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = new FormGroup({
      email: new FormControl('munir.sheikh@live.com', [Validators.required, Validators.email]),
      password: new FormControl('Pa$$w0rd', Validators.required),
    })
  }

  onSubmit(){
    if(this.loginForm !== undefined) {
      this.accountService.login(this.loginForm.value).subscribe(() => {
        if(this.user?.roles.includes('Candidate')) return;
        this.router.navigateByUrl('userTask');
      }, error => {
        console.log(error);
      })
    }
  }


}
