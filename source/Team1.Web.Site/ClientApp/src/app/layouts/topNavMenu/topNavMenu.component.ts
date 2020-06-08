import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'app.common/services/auth.service';

import { ITokenUser } from 'app.common/dtos/TokenUserDto';

@Component({
    selector: 'topNavMenu',
    templateUrl: './topNavMenu.component.html',
    styleUrls: ['./topNavMenu.component.scss']
})
export class TopNavMenuComponent {
  constructor(private authService: AuthService, private router: Router) { }

  isBurgerMenuExpanded: boolean = false;
  isUserProfileExpanded: boolean = false;

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  get userInfo(): ITokenUser {
    return this.authService.userInfo || {};
  }

  get isImpersonating(): boolean {
    return this.userInfo.isImpersonatingCompany || this.userInfo.isImpersonatingUser || false;
  }

  get impersonatingWhom(): string {
    if (!this.isImpersonating) {
      return "";
    }

    var result = "";
    if (this.userInfo.isImpersonatingCompany) {
      result += this.userInfo.companyName;
    }

    if (this.userInfo.isImpersonatingUser) {
      result += `${result} ${this.userInfo.firstName} ${this.userInfo.lastName}`;
    }

    return result;
  }

  endImpersonation() {
    this.authService.impersonate(undefined, undefined);
  }

    logout = () => {
      var self = this;
      this.authService.logout(false).subscribe((data: any) => {
          self.router.navigate(["/"]);
        },
        error => {
        },
        () => {
        });
    }

    public logo = require("assets/logo.png");
}
