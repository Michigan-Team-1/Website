import { Component, OnInit } from '@angular/core';
import { Router  } from '@angular/router';

import { escapeRegExp } from 'app.common/helpers/general';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { AuthService } from 'app.common/services/auth.service';

import { IChangePassword, IChangePassword_PropertyAttributes } from 'app.common/dtos/AccountDtos';
import { BsModalRef } from 'ngx-bootstrap';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'changePassword',
  templateUrl: './changePassword.component.html',
})
export class ChangePasswordComponent implements OnInit {
  isBusy: boolean = false;
    message: string | undefined;
  routeUrl: string = "/";
  public dto: IChangePassword = {};
  public dtoPropertyAttributes = IChangePassword_PropertyAttributes;

    constructor(
        private router: Router,
      private activeModal: BsModalRef,
        private authService: AuthService
    ) {
    }

    ngOnInit() {
    }

    get passwordEscaped() {
      if (this.dto.password == null) {
        return null;
      }

      return escapeRegExp(this.dto.password);
    }

  save(f: NgForm) {
    if (f.invalid) {
      return;
    }
        this.isBusy = true;

        this.authService.changePassword(this.dto)
            .subscribe(
            (data) => {
                this.message = "";
              this.isBusy = false;
              this.activeModal.hide();
            },
            (error: any) => {
                this.message = getErrorMessageFromServerResponse(error);
                this.isBusy = false;
            });
  }

  close(closeMethod: string) {
    this.activeModal.hide();
  }

   
}
