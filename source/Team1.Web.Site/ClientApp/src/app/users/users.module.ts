import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CgBusyModule } from 'angular-busy2';

import { SmartTableModule } from 'smart-table-ng';

import { UsersComponent } from './users.component';
import { UserAddEditComponent } from './userAddEdit.component';
import { UsersService } from './users.service';

import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ComponentsModule } from 'app.common/components';



@NgModule({
  imports: [FormsModule, CommonModule, PipesModule, DirectivesModule, CollapseModule,
    BsDropdownModule, BsDatepickerModule, ModalModule, CgBusyModule, ComponentsModule, SmartTableModule],
    declarations: [UsersComponent, UserAddEditComponent],
    exports: [UsersComponent, UserAddEditComponent],
    providers: [UsersService],
    entryComponents: [UserAddEditComponent]
})
export class UsersModule { }
