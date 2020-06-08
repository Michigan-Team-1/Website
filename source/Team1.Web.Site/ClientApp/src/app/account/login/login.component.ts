import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { AuthService } from 'app.common/services/auth.service';

import { ILogin, ILogin_PropertyAttributes } from 'app.common/dtos/AccountDtos';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  constructor(
    private router: Router,
    private authService: AuthService
  ) {
  }

  dto: ILogin = { email: undefined, password: undefined };
  dtoPropertyAttributes = ILogin_PropertyAttributes;
  logo = require("assets/logo.png");
  googlePlay = require("assets/Google_Play.png");
  appStore = require("assets/App_Store.png");
  isBusy: boolean = false;
  message: string | undefined;
  popOverHtml: string = "<ul><li>At least 1 lower case</li><li>At leats 1 upper case</li></ul>";

  ngOnInit() {
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/']);
    }

    var self = this;
    if (this.authService.refreshTokenObservable != null) {
      this.isBusy = true;
      this.authService.refreshTokenObservable.subscribe((data: any) => {
        self.isBusy = false;
        let url = this.authService.getRedirectUrl();
        self.router.navigate([url]);
      },
        (error: any) => {
          self.isBusy = false;
        },
        () => {
        });
    }
  }

  submit() {
    this.isBusy = true;
    var self = this;
    this.authService.login(this.dto)
      .subscribe(
      (data: any) => {
        self.isBusy = false;
          self.message = undefined;
        },
      (error: any) => {
        self.isBusy = false;
          self.message = getErrorMessageFromServerResponse(error);
      },
      () => {
        if (self.authService.isAuthenticated) {
          let url = this.authService.getRedirectUrl();
          self.router.navigate([url]);
        }
      }
    );
  }
}
