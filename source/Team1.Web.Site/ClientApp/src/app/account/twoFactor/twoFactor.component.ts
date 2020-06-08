import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { TwoFactorAppsComponent } from '../twoFactorApps/twoFactorApps.component';

import { AuthService } from 'app.common/services/auth.service';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { ITwoFactorCode, ITwoFactorCode_PropertyAttributes } from 'app.common/dtos/AccountDtos';

@Component({
  selector: 'twoFactor',
  templateUrl: './twoFactor.component.html',
})
export class TwoFactorComponent {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
  ) { }

  isBusy: boolean = false;
  message: string | undefined;
  dto: ITwoFactorCode = {};
  dtoPropertyAttributes = ITwoFactorCode_PropertyAttributes;
  logo = require("assets/logo.png");

  ngOnInit() {
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/']);
    }
  }

  submit() {
    var self = this;
    this.isBusy = true;
    this.authService.loginWith2fa(this.dto).subscribe(
      (data: any) => {
        self.message = undefined;
        self.isBusy = false;
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
