import { Component, AfterViewInit, OnInit, ElementRef, Inject, ChangeDetectorRef } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AuthService } from 'app.common/services/auth.service';

import { escapeRegExp } from 'app.common/helpers/general';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";

import { IRegister, IRegister_PropertyAttributes } from 'app.common/dtos/AccountDtos';
import { certificationLevels } from 'app.common/constants';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { setIsUpdatedIfChanged } from 'app.common/helpers/object';
import { moment } from 'ngx-bootstrap/chronos/test/chain';

declare var grecaptcha: any;

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
})
export class RegisterComponent implements OnInit, AfterViewInit {
  constructor(
    @Inject(DOCUMENT) private document: Document,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private elementRef: ElementRef,
    private changeDetectorRef: ChangeDetectorRef,
  ) { }

  isBusy: boolean = false;
  returnUrl: string = '/';
  message: string | undefined;
  submitMessage: string | undefined;
  disableSubmit: boolean = false;
  retryCount: number = 0;
  retryCountMax: number = 3;
  dto: IRegister = {
    certificationLevel: 0
  };
  dtoPropertyAttributes = IRegister_PropertyAttributes;
  certLevels:Array<ISelectOption<number>> = certificationLevels;
  logo = require("assets/logo.png");
  addressIsRequired = true;
  maxBirthDate = new Date();

  ngOnInit() {
    this.maxBirthDate.setFullYear(this.maxBirthDate.getFullYear() - 16);
    if (!this.dto.addresses || this.dto.addresses.length == 0) {
      this.addAddress();
    }

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (this.authService.isAuthenticated) {
      this.router.navigate([this.returnUrl]);
    }
  }

  ngAfterViewInit() {
    const s = this.document.createElement('script');
    s.type = 'text/javascript';
    s.src = 'https://www.google.com/recaptcha/api.js?render=6LfUFlkUAAAAACLaSifE3SuvGOh5vnLYWjmiayUX';
    const self = this;
    s.onload = function () { self.ExecuteReCaptcha(); };
    this.elementRef.nativeElement.appendChild(s);
  }

  ngOnDestroy() {
    this.disableSubmit = true;
    var elements = this.document.getElementsByClassName("grecaptcha-badge");
    if (elements != null && elements.length > 0)
      elements[0]!.remove();
  }

  ExecuteReCaptcha() {
    this.retryCount = 0;
    this.disableSubmit = true;
    if (this.submitMessage == null) this.submitMessage = "Can't submit yet.  Google ReCaptcha is loading...";
    this.changeDetectorRef.detectChanges();
    grecaptcha.ready(() => {
      grecaptcha.execute('6LfUFlkUAAAAACLaSifE3SuvGOh5vnLYWjmiayUX', { action: 'register' }).then((token: any) => {
        this.dto.recaptchaToken = token;
        this.disableSubmit = false;
        this.submitMessage = undefined;
        this.changeDetectorRef.detectChanges();
      }, (error: any) => {
        console.log("Google ReCaptcha:  " + error);
        this.retryCount++;
        if (this.retryCount <= this.retryCountMax) {
          this.ExecuteReCaptcha();
        }
        else {
          this.ngOnDestroy();
          this.ngAfterViewInit();
          this.submitMessage = 'Google ReCaptcha is having a problem.  Attempting to completely reload it.';
        }
      });
    });
  }

  addAddress() {
    if (this.dto.addresses == null) this.dto.addresses = [];
    this.dto.addresses.push({
      addressObj: {},
      isActive: true,
      isUpdated: false,
    });
  }

  removeAddress(addressIndex: number) {
    if (this.dto.addresses == null) return;

    var address = this.dto.addresses[addressIndex];
    if (!address.addressId) {
      this.dto.addresses.splice(addressIndex, 1);
      return;
    }

    address.isDeleted = true;
  }

  get passwordEscaped() {
    if (this.dto.password == null) {
      return null;
    }

    return escapeRegExp(this.dto.password);
  }

  get emailEscaped() {
    if (this.dto.email == null) {
      return null;
    }

    return escapeRegExp(this.dto.email);
  }

  submit() {
    this.message = undefined;
    if (this.dto.addresses == null || this.dto.addresses.every((item, index) => { return item.isDeleted!; })) {
      this.message = "Address is required."
      return;
    }

    setIsUpdatedIfChanged(this.dto, { });

    this.isBusy = true;
    this.authService.register(this.dto).subscribe(
      (data: any) => {
        this.isBusy = false;
        this.router.navigate([this.returnUrl]);
      },
      (error: any) => {
        this.message = getErrorMessageFromServerResponse(error);
        this.isBusy = false;
        this.ngOnDestroy();
        this.ngAfterViewInit();
      });
  }
}
