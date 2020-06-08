import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { UsersService } from './users.service';
import { CommonService } from 'app.common/services/common.service';

import { IUser, IUser_PropertyAttributes } from 'app.common/dtos/UserDto';
import { IRole } from 'app.common/dtos/RoleDto';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { MemberTypeEnum_class } from 'app.common/enums/MemberTypeEnum';
import { IUserMemberType } from 'app.common/dtos/UserMemberTypeDto';
import { certificationLevels } from 'app.common/constants';

@Component({
  selector: 'userAddEdit',
  templateUrl: './userAddEdit.component.html',
})
export class UserAddEditComponent implements OnInit {
  constructor(private usersService: UsersService,
    private activeModal: BsModalRef,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: IUser | undefined;
  defaultDto: IUser = { isActive: true, isUpdated: true, certificationLevel: 0 };
  dto: IUser = deepClone(this.defaultDto);
  dtoPropertyAttributes = IUser_PropertyAttributes;
  roles: Array<IRole> | undefined;
  selectedRole: IRole | undefined;
  memberType: ISelectOption<number> | undefined;
  memberTypes: Array<ISelectOption<number>> = MemberTypeEnum_class.enumAsSelectOptions;
  certLevels: Array<ISelectOption<number>> = certificationLevels;
  message: string | undefined;
  success: boolean = false;
  addressIsRequired: boolean = true;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;

  ngOnInit() {
    this.addressIsRequired = this.dtoPropertyAttributes.addresses_Attributes.required.value;

    if (this.dtoOriginal == null) this.dtoOriginal = deepClone(this.defaultDto);
    this.dto = this.dtoOriginal != null ? deepClone(this.dtoOriginal) : this.dto;

    this.isBusy++;
    this.usersService.getGrantableRoles().subscribe((data) => {
      this.roles = data;
      this.isBusy--;
    }, (error) => { this.isBusy--; }, () => { });

    if (this.dto.userId != null && this.dto.userId > 0 && this.dto.addresses == null) {
      this.dto.addresses = [];
      this.isBusy++;
      this.commonService.getUserAddresses(this.dto.userId)
        .subscribe((data) => {
          this.isBusy--;
          if (data.length > 0) {
            if (this.dtoOriginal != null) {
              this.dtoOriginal.addresses = data;
            }
            this.dto.addresses = deepClone(data);
          }
          else {
            if (this.dtoOriginal != null) this.dtoOriginal.addresses = [];
            this.dto.addresses = [];
          }
        }, (error: any) => { this.isBusy--; }, () => { });
    }
  }

  roleCompareWith(o1: IRole, o2: IRole): boolean {
    return o1 != null && o2 != null && o1.roleId == o2.roleId;
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

  getRoles() {
    if (this.dto.roles == null || this.dto.roles.length == 0 || this.roles == null) return this.roles;
    let self = this;
    return this.roles.filter((value) => {
      return !self.dto.roles!.some((item) => { return item.roleId == value.roleId; });
    });
  }

  addRole() {
    if (this.selectedRole == null) return;
    if (this.dto.roles == null) this.dto.roles = [];
    var role: IRole = deepClone(this.selectedRole);
    role.isAdded = true;
    this.dto.roles.push(role);

    this.selectedRole = undefined;
  }

  removeRole(roleIndex: number) {
    if (this.dto.roles == null) return;

    var role = this.dto.roles[roleIndex];
    if (role.isAdded) {
      this.dto.roles.splice(roleIndex, 1);
      return;
    }

    role.isDeleted = true;
  }

  getMemberTypes() {
    if (this.dto.userMemberTypes == null || this.dto.userMemberTypes.length == 0) return this.memberTypes;
    let self = this;
    return this.memberTypes.filter((value) => {
      return !self.dto.userMemberTypes!.some((item) => { return item.memberTypeId == value.value; });
    });
  }

  addMemberType() {
    if (this.memberType == null) return;
    if (this.dto.userMemberTypes == null) this.dto.userMemberTypes = [];
    var memberType: IUserMemberType = { isAdded: true, memberTypeId: this.memberType.value, memberTypeString: this.memberType.text };
    this.dto.userMemberTypes.push(memberType);

    this.memberType = undefined;
  }

  removeMemberType(index: number) {
    if (this.dto.userMemberTypes == null) return;

    var userMemberType = this.dto.userMemberTypes[index];
    if (userMemberType.isAdded) {
      this.dto.userMemberTypes.splice(index, 1);
      return;
    }

    userMemberType.isDeleted = true;
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

    this.isBusy++;
    this.usersService.saveUser(this.dto).subscribe((data) => {
      this.isBusy--;
      this.dto = data;
      this.success = true;
      this.activeModal.hide();
    }, (error) => {
      this.isBusy--;
      this.message = getErrorMessageFromServerResponse(error);
    }, () => {
    });
  }

  close(closeMethod: string) {
    this.activeModal.hide();
  }
}
