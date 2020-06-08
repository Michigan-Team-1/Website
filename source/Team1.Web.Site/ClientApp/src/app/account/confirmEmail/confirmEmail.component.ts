import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute  } from '@angular/router';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { AuthService } from 'app.common/services/auth.service';

import { IConfirmEmail } from 'app.common/dtos/AccountDtos';

@Component({
    selector: 'confirmEmail',
    templateUrl: './confirmEmail.component.html',
})
export class ConfirmEmailComponent implements OnInit {
  isBusy: Array<any> = [];
    loading = false;
    message: string | undefined;
    routeUrl: string = "/";

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private authService: AuthService
    ) {
    }

    ngOnInit() {
      this.activatedRoute.queryParams.subscribe(params => { this.dto.code = params["code"] || params["Code"]; });
      this.activatedRoute.queryParams.subscribe(params => { this.dto.userId = params["userid"] || params["userId"] || params["UserId"]; });
      this.confirmEmail();
    }

    confirmEmail() {
        this.loading = true;

        this.isBusy.push(this.authService.confirmEmail(this.dto)
            .subscribe(
            (data: any) => {
                let url = this.authService.getRedirectUrl();
                this.message = undefined;
            },
            (error: any) => {
                this.message = getErrorMessageFromServerResponse(error);
                this.loading = false;
            }));
    }

    public dto: IConfirmEmail = {};
    public logo = require("assets/logo.png");
}
