import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute  } from '@angular/router';

import { escapeRegExp } from 'app.common/helpers/general';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { AuthService } from 'app.common/services/auth.service';

import { IResetPassword, IResetPassword_PropertyAttributes } from 'app.common/dtos/AccountDtos';

@Component({
    selector: 'resetPassword',
    templateUrl: './resetPassword.component.html',
})
export class ResetPasswordComponent implements OnInit {
  isBusy: boolean = false;
    message: string | undefined;
    routeUrl: string = "/";

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private authService: AuthService
    ) {
    }

    ngOnInit() {
      this.activatedRoute.queryParams.subscribe(params => { this.dto.code = params["code"] || params["Code"]; })
    }

    get passwordEscaped() {
      if (this.dto.password == null) {
        return null;
      }

      return escapeRegExp(this.dto.password);
    }

    submit() {
        this.isBusy = true;

        this.authService.resetPassword(this.dto)
            .subscribe(
            (data: any) => {
                let url = this.authService.getRedirectUrl();
                this.message = "";
                console.log(url);
                this.isBusy = false;
                this.router.navigate([this.routeUrl]);
            },
            (error: any) => {
                this.message = getErrorMessageFromServerResponse(error);
                this.isBusy = false;
            });
    }

    public dto: IResetPassword = {};
    public dtoPropertyAttributes = IResetPassword_PropertyAttributes;
    public logo = require("assets/logo.png");
}
