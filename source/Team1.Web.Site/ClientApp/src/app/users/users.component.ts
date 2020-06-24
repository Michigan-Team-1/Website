import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { UserAddEditComponent } from './userAddEdit.component'
import { UsersService } from './users.service';

import { IUser, IUser_PropertyAttributes } from 'app.common/dtos/UserDto';
import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, from } from 'smart-table-ng';
import * as crud from 'smart-table-crud';

@Component({
  selector: 'users',
  templateUrl: './users.component.html',
  providers: [{
    provide: SmartTable,
    useFactory: (usersService: UsersService) => from(usersService.getUsers(), {
      search: {},
      sort: { pointer: "lastName", direction: "asc" },
      filter: {
        isActive: [{ operator: "equals", type: "string", value: "true" }]
      },
      slice: { page: 1, size: defaultPageSize }
    }, <any>crud),
    deps: [UsersService]
  }]
})
export class UsersComponent implements OnInit {
  constructor(private usersService: UsersService, private modalService: BsModalService,
    public authService: AuthService, private table: SmartTable<IUser>
  ) { }

  isCollapsedFilters: boolean = true;
  isBusy:boolean = false;
  dtoPropertyAttributes = IUser_PropertyAttributes;
  showAdminOptions: boolean = this.authService.isAuthenticated && this.authService.userAddEditDelete;
  columnCount: number = 5;

  ngOnInit() {
    if (this.showAdminOptions) {
      this.columnCount = 8;
    }
  }

  sendPasswordResetEmail = (userId: number, isnew: boolean) => {
    this.isBusy = true;
    this.usersService.sendPasswordResetEmail(userId, isnew).subscribe(() => { this.isBusy = false; }, () => { this.isBusy = false; }, () => { });
  };

  edit(index: number | undefined) {
    const initialState = {
      dtoOriginal: index != null ? (<any>this.table).get(index) : undefined
    };

    let subcription = this.modalService.onHide.subscribe((reason: string) => {
      if (modal.content.success) {
        let dto = modal.content.dto;
        if (index == null) {
          (<any>this.table).insert(dto);
        }
        else {
          (<any>this.table).update(index, dto);
        }
      }

      subcription.unsubscribe();
    });

    let modal: BsModalRef = this.modalService.show(UserAddEditComponent, { initialState, class: 'modal-lg' });
  }

  impersonate(companyId: number, userId: number) {
    this.isBusy = true;
    this.authService.impersonate(companyId, userId).subscribe(() => { this.isBusy = false; }, () => { this.isBusy = false; }, () => { });
  }
}
