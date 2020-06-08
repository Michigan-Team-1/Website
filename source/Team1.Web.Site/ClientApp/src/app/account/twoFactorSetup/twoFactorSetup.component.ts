import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { TwoFactorAppsComponent } from '../twoFactorApps/twoFactorApps.component';

import { AuthService } from 'app.common/services/auth.service';
import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { ITwoFactorSetup, ITwoFactorSetup_PropertyAttributes } from 'app.common/dtos/AccountDtos';

@Component({
  selector: 'twoFactorSetup',
  templateUrl: './twoFactorSetup.component.html',
})
export class TwoFactorSetupComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
  ) { }

  dto: ITwoFactorSetup = {};
  dtoPropertyAttributes = ITwoFactorSetup_PropertyAttributes;
  logo = require("assets/logo.png");
  isBusy: boolean = false;
  message: string | undefined;

  ngOnInit() {
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/']);
    }

    if (this.authService.userInfo != null) {
      this.dto.phoneNumber = this.authService.userInfo.phoneNumber;
    }
  }

  submit() {
    var self = this;
    this.isBusy = true;
    this.authService.setup2fa(this.dto).subscribe(
      (data: any) => {
        self.message = undefined;
        self.isBusy = false;
      },
      (error: any) => {
        self.message = getErrorMessageFromServerResponse(error);
        self.isBusy = false;
      });
  }
}
