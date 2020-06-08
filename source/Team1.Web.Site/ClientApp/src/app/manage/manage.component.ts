import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { AuthService } from 'app.common/services/auth.service';
import { IUserProfile, IUserProfile_PropertyAttributes } from 'app.common/dtos/UserProfileDto';
import { ManageService } from './manage.service';
import { NgForm } from '@angular/forms';
import { setIsUpdatedIfChanged, deepClone } from 'app.common/helpers/object';
import { getErrorMessageFromServerResponse } from 'app.common/helpers/general';
import { ChangePasswordComponent } from './changePassword.component';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { certificationLevels } from 'app.common/constants';

@Component({
  selector: 'users',
  templateUrl: './manage.component.html',
})
export class ManageComponent implements OnInit {
  constructor(private manageService: ManageService, private modalService: BsModalService,
    private authService: AuthService
  ) { }

  isBusy:boolean = false;
  dtoPropertyAttributes = IUserProfile_PropertyAttributes;
  defaultDto: IUserProfile = { isUpdated: false };
  dtoOriginal: IUserProfile = deepClone(this.defaultDto);
  dto: IUserProfile = deepClone(this.defaultDto);
  certLevels: Array<ISelectOption<number>> = certificationLevels;
  message: string | undefined;
  success: boolean = false;
  addressIsRequired: boolean = true;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;

  ngOnInit() {
    this.addressIsRequired = this.dtoPropertyAttributes.addresses_Attributes.required.value;

    this.isBusy = true;
    this.manageService.getUserProfile()
      .subscribe((data) => {
        this.isBusy = false;
        this.dtoOriginal = data;
        this.dto = deepClone(this.dtoOriginal);
      }, (error: any) => { this.isBusy = false; }, () => { });
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

  ngDoCheck() {
    this.dataIsUpdated();
  }

  dataIsUpdated(): boolean {
    this.dtoUpdated = setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.dataUpdated = this.dtoUpdated;
    return this.dataUpdated;
  }

  save(form: NgForm) {
    if (!form.valid) {
      return;
    }

    this.message = undefined;
    if (this.dto.addresses == null || this.dto.addresses.every((item, index) => { return item.isDeleted!; })) {
      this.message = "Address is required."
      return;
    }

    setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.isBusy = true;
    this.manageService.saveUserProfile(this.dto).subscribe((data) => {
      this.isBusy = false;
      this.dtoOriginal = data;
      this.dto = deepClone(this.dtoOriginal);
    }, (error) => {
      this.isBusy = false;
      this.message = getErrorMessageFromServerResponse(error);
    }, () => {
    });
  }

  changePassword() {
    
    let subcription = this.modalService.onHide.subscribe((reason: string) => {
      subcription.unsubscribe();
    });

    let modal: BsModalRef = this.modalService.show(ChangePasswordComponent);
  }
}
