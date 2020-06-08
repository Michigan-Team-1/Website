import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";

import { AuthService } from 'app.common/services/auth.service';

import { IForgotPassword, IForgotPassword_PropertyAttributes } from 'app.common/dtos/AccountDtos';

@Component({
    selector: 'forgotPassword',
    templateUrl: './forgotPassword.component.html',
})
export class ForgotPasswordComponent implements OnInit {
  isBusy: boolean = false;
    returnUrl: string = "/";
    message: string | undefined;

    constructor(
        private router: Router,
        private authService: AuthService
    ) { }

    ngOnInit() {
      if (this.authService.isAuthenticated) {
        this.router.navigate([this.returnUrl]);
      }
    }

    submit() {
      this.isBusy = true;
        this.authService.forgotPassword(this.dto)
            .subscribe(
          (data: any) => {
            this.isBusy = false;
                this.router.navigate([this.returnUrl]);
            },
          (error: any) => {
            this.message = getErrorMessageFromServerResponse(error);
              this.isBusy = false;
            });
    }

    public dto: IForgotPassword = {};
    public dtoPropertyAttributes = IForgotPassword_PropertyAttributes;
    public logo = require("assets/logo.png");
}
