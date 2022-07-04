import { Component, OnInit, ElementRef, Inject, ChangeDetectorRef } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';

import { AuthService } from 'app.common/services/auth.service';

import { escapeRegExp } from 'app.common/helpers/general';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";

import { IRegister, IRegister_PropertyAttributes } from 'app.common/dtos/AccountDtos';
import { certificationLevels } from 'app.common/constants';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { setIsUpdatedIfChanged } from 'app.common/helpers/object';
import { CommonService } from 'app.common/services/common.service';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
})
export class RegisterComponent implements OnInit {
  
  constructor(
    @Inject(DOCUMENT) private document: Document,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private elementRef: ElementRef,
    private changeDetectorRef: ChangeDetectorRef,
    private commonService: CommonService
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
  dtoOriginal: IRegister = {
    certificationLevel: 0
  };
  dtoPropertyAttributes = IRegister_PropertyAttributes;
  certLevels:Array<ISelectOption<number>> = certificationLevels;
  logo = require("assets/logo.png");
  addressIsRequired = true;
  maxBirthDate = new Date();
  recaptchaSiteKey: string = "6LfoE6MZAAAAAF50jKHFmtwZnpQGzyD56VQGVxx5";
  mobileCarriers: Array<ISelectOption<number>> = [];

  ngOnInit() {
    this.maxBirthDate.setFullYear(this.maxBirthDate.getFullYear() - 16);
    if (!this.dto.addresses || this.dto.addresses.length == 0) {
      this.addAddress();
    }

    this.isBusy = true;
    this.commonService.getMobileCarriers().subscribe((data) => {
      this.isBusy = false;
      this.mobileCarriers = data;
    }, (error: any) => {
      this.isBusy = false;
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (this.authService.isAuthenticated) {
      this.router.navigate([this.returnUrl]);
    }
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
      });
  }
}
