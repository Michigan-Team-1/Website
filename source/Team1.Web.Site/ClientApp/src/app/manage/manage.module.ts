import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CgBusyModule } from 'angular-busy2';

import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ComponentsModule } from 'app.common/components';
import { ManageComponent } from './manage.component';
import { ManageService } from './manage.service';
import { ChangePasswordComponent } from './changePassword.component';

@NgModule({
  imports: [FormsModule, CommonModule, PipesModule, DirectivesModule, CollapseModule, BsDropdownModule, BsDatepickerModule, ModalModule, CgBusyModule, ComponentsModule],
  declarations: [ManageComponent, ChangePasswordComponent],
  exports: [ManageComponent, ChangePasswordComponent],
  providers: [ManageService],
  entryComponents: [ChangePasswordComponent]
})
export class ManageModule { }
